﻿using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Pegasus.Library.Api;
using Pegasus.Library.Models.Manage;
using Pegasus.Services;
using ManageControllerStrings = Pegasus.Library.Services.Resources.Resources.ControllerStrings.ManageController;


namespace Pegasus.Controllers
{
    [Authorize(Roles="PegasusUser")]
    [Route("Account/Manage")]
    public class ManageController : Controller
    {
        private readonly IManageEndpoint _manageEndpoint;
        private readonly ISignInManager _signInManager;
        private readonly ILogger<ManageController> _logger;

        public ManageController(IManageEndpoint manageEndpoint, ISignInManager signInManager, ILogger<ManageController> logger)
        {
            _manageEndpoint = manageEndpoint;
            _signInManager = signInManager;
            _logger = logger;
        }

        private string UserId
        {
            get { return User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value; }
        }

        [Route(nameof(ChangePassword))]
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var hasPasswordModel = await _manageEndpoint.HasPasswordAsync(UserId);
            if (hasPasswordModel.HasErrors)
            {
                LogErrors(hasPasswordModel);
                ModelState.AddModelError(string.Empty, ManageControllerStrings.CannotChangePassword);
                return View();
            }

            if (!hasPasswordModel.HasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            return View();
        }

        [Route(nameof(ChangePassword))]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.UserId = UserId;

            var changePasswordResult = await _manageEndpoint.ChangePasswordAsync(model);
            if (!changePasswordResult.Succeeded)
            {
                _logger.LogError("Failed to change password for User with ID '{UserId}'.", UserId);
                LogErrors(changePasswordResult);
                ModelState.AddModelError(string.Empty, ManageControllerStrings.FailedToChangePassword);
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(UserId);

            _logger.LogInformation("User with UserId {UserId} changed their password successfully.", UserId);
            model = new ChangePasswordModel
            {
                StatusMessage = ManageControllerStrings.PasswordChangedSuccess
            };

            return View(model);
        }

        [Route(nameof(Disable2Fa))]
        [HttpGet]
        public async Task<IActionResult> Disable2Fa()
        {
            var twoFactorAuthentication = await _manageEndpoint.GetTwoFactorEnabledAsync(UserId);

            var model = new Disable2FaModel();
            if (!twoFactorAuthentication.IsEnabled)
            {
                model.StatusMessage = ManageControllerStrings.CannotDisable2Fa;
            }

            return View(model);
        }

        [Route(nameof(Disable2Fa))]
        [HttpPost]
        public async Task<IActionResult> Disable2Fa(Disable2FaModel model)
        {
            var twoFactorAuthentication = await _manageEndpoint.GetTwoFactorEnabledAsync(UserId);
            if (!twoFactorAuthentication.IsEnabled)
            {
                model.StatusMessage = ManageControllerStrings.CannotDisable2Fa;
                return View(model);
            }

            var setTwoFactorEnabled = new SetTwoFactorEnabledModel
            {
                UserId = UserId,
                SetEnabled = false
            };

            var disable2FaResult = await _manageEndpoint.SetTwoFactorEnabledAsync(setTwoFactorEnabled);
            if (!disable2FaResult.Succeeded)
            {
                _logger.LogError("Failed to disable 2fa for User with ID '{UserId}'.", UserId);
                LogErrors(disable2FaResult);
                model.StatusMessage = ManageControllerStrings.FailedToDisable2Fa;
                return View(model);
            }
            
            _logger.LogTrace("User with ID '{UserId}' has disabled 2fa.", UserId);
            return RedirectToAction("TwoFactorAuthentication", "Manage");
        }

        [Route(nameof(EnableAuthenticator))]
        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var enableAuthenticatorModel = await LoadEnableAuthenticatorModel(UserId);
            if (enableAuthenticatorModel.HasErrors)
            {
                LogErrors(enableAuthenticatorModel);
                ModelState.AddModelError(string.Empty,  ManageControllerStrings.CannotEnableAuthenticator);
            }

            return View(enableAuthenticatorModel);
        }

        [Route(nameof(EnableAuthenticator))]
        [HttpPost]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorModel model)
        {
            if (!ModelState.IsValid)
            {
                return await EnableAuthenticator();
            }

            var verifyTwoFactorTokenModel = new VerifyTwoFactorTokenModel
            {
                UserId = UserId,
                VerificationCode = model.Code?.Replace(" ", string.Empty).Replace("-", string.Empty)
            };

            verifyTwoFactorTokenModel = await _manageEndpoint.VerifyTwoFactorTokenAsync(verifyTwoFactorTokenModel);
            if (verifyTwoFactorTokenModel.HasErrors)
            {
                LogErrors(verifyTwoFactorTokenModel);
                ModelState.AddModelError(string.Empty, ManageControllerStrings.CannotEnableAuthenticatorVerifyToken);
                return View(model);
            }
            if (!verifyTwoFactorTokenModel.IsTokenValid)
            {
                ModelState.AddModelError(string.Empty, ManageControllerStrings.CannotEnableAuthenticatorTokenInvalid);
                return await EnableAuthenticator();
            }

            await _signInManager.DoTwoFactorSignInAsync(UserId, false);

            var setTwoFactorEnabledModel = new SetTwoFactorEnabledModel
            {
                UserId = UserId,
                SetEnabled = true
            };
            var set2FaEnabled = await _manageEndpoint.SetTwoFactorEnabledAsync(setTwoFactorEnabledModel);
            if (set2FaEnabled.HasErrors)
            {
                _logger.LogError("Failed to enable 2FA with an authenticator app for User with ID '{UserId}'.", set2FaEnabled.UserId);
                LogErrors(set2FaEnabled);
                ModelState.AddModelError(string.Empty, ManageControllerStrings.CannotEnableAuthenticatorSet2FaEnabled);
                return View(model);
            }

            _logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", set2FaEnabled.UserId);
            model.StatusMessage = ManageControllerStrings.AuthenticatorAppVerified;

            var recoveryCodeStatus = new RecoveryCodeStatusModel {UserId = UserId};
            recoveryCodeStatus = await _manageEndpoint.CheckRecoveryCodesStatus(recoveryCodeStatus);
            if (recoveryCodeStatus.HasErrors)
            {
                _logger.LogError("Failed to check recovery codes for User with ID '{UserId}'.", set2FaEnabled.UserId);
                LogErrors(recoveryCodeStatus);
                ModelState.AddModelError(string.Empty, ManageControllerStrings.CannotEnableAuthenticatorRecoveryCodes);
                return View(model);
            }
            if (recoveryCodeStatus.IsUpdated)
            {
                var showRecoveryCodesModel = new ShowRecoveryCodesModel()
                {
                    StatusMessage = model.StatusMessage,
                    RecoveryCodes = recoveryCodeStatus.RecoveryCodes?.ToArray()
                };
                return RedirectToAction("ShowRecoveryCodes", showRecoveryCodesModel);
            }

            return RedirectToAction("TwoFactorAuthentication");
        }

        [Route(nameof(GenerateRecoveryCodes))]
        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var check2Fa = await CheckTwoFactorIsEnabled();
            return check2Fa ?? View();
        }

        [Route(nameof(GenerateRecoveryCodes))]
        [HttpPost]
        public async Task<IActionResult> GenerateRecoveryCodesPost()
        {
            var check2Fa = await CheckTwoFactorIsEnabled();
            if (check2Fa != null) 
            {
                return check2Fa;
            }

            var generateCodesModel = await _manageEndpoint.GenerateNewRecoveryCodesAsync(UserId);
            if (generateCodesModel.HasErrors) return HasErrors(generateCodesModel, nameof(GenerateRecoveryCodes), ManageControllerStrings.CannotGenerateRecoveryCodes);

            var showRecoveryCodes = new ShowRecoveryCodesModel
            {
                RecoveryCodes = generateCodesModel.RecoveryCodes?.ToArray()
            };

            _logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", UserId);

            return RedirectToAction("ShowRecoveryCodes", showRecoveryCodes);
        }
        
        [Route(nameof(Index))]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _manageEndpoint.GetUserDetails(UserId);
            LogErrors(model);
            return View(model);
        }

        [Route(nameof(Index))]
        [HttpPost]
        public async Task<IActionResult> Index(UserDetailsModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Username = User.Identity.Name;
                return View(model);
            }

            model = await _manageEndpoint.SetUserDetails(model);
            if (model.HasErrors)
            {
                LogErrors(model);
                return View(model);
            }

            model.StatusMessage = "User details were updated.";
            return View(model);
        }

        [Route(nameof(ResetAuthenticator))]
        [HttpGet]
        public IActionResult ResetAuthenticator()
        {
            return View();
        }

        [Route(nameof(ResetAuthenticator))]
        [HttpPost]
        // ReSharper disable once UnusedParameter.Global
        public async Task<IActionResult> ResetAuthenticator(ResetAuthenticatorModel model)
        {
            model.UserId = UserId;
            model = await _manageEndpoint.ResetAuthenticatorAsync(model);
            if (model.HasErrors)
            {
                LogErrors(model);
                return View(model);
            }
            
            _logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", model.UserId);
            var enableAuthenticatorModel = new EnableAuthenticatorModel
            {
                StatusMessage =
                    "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key."
            };


            return RedirectToAction("EnableAuthenticator", "Manage", enableAuthenticatorModel);
        }
                
        [Route(nameof(SetPassword))]
        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var model = await _manageEndpoint.HasPasswordAsync(UserId);
            if (model.HasErrors)
            {
                LogErrors(model);
                return View();
            }

            if (model.HasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            return View();
        }

        [Route(nameof(SetPassword))]
        [HttpPost]
        public async Task<IActionResult> SetPassword(SetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.UserId = UserId;

            var addPasswordResult = await _manageEndpoint.AddPasswordAsync(model);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(model.UserId);

            model = new SetPasswordModel {StatusMessage = "Your password has been set."};
            return View(model);
        }

        [Route(nameof(ShowRecoveryCodes))]
        [HttpGet]
        public IActionResult ShowRecoveryCodes(ShowRecoveryCodesModel model)
        {
            if (model.RecoveryCodes == null || model.RecoveryCodes.Length == 0)
            {
                return RedirectToAction("TwoFactorAuthentication");
            }

            return View(model);
        }

        [Route(nameof(TwoFactorAuthentication))]
        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var model = await _manageEndpoint.TwoFactorAuthentication(UserId);
            model.IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(UserId);
            return View(model);
        }

        [Route(nameof(TwoFactorAuthentication))]
        [HttpPost]
        // ReSharper disable once RedundantAssignment
        public async Task<IActionResult> TwoFactorAuthentication(TwoFactorAuthenticationModel model)
        {
            await _signInManager.ForgetTwoFactorClientAsync();
            model = await _manageEndpoint.TwoFactorAuthentication(UserId);
            model.StatusMessage = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
            return View(model);
        }

        private async Task<EnableAuthenticatorModel> LoadEnableAuthenticatorModel(string userId)
        {
            var authenticatorKeyModel =  await _manageEndpoint.LoadSharedKeyAndQrCodeUriAsync(userId);
            var model = new EnableAuthenticatorModel
            {
                Errors = authenticatorKeyModel.Errors,
                SharedKey = authenticatorKeyModel.SharedKey, 
                AuthenticatorUri = authenticatorKeyModel.AuthenticatorUri
            };
            return model;
        }

        private async Task<IActionResult> CheckTwoFactorIsEnabled()
        {
            var twoFactorAuthentication = await _manageEndpoint.GetTwoFactorEnabledAsync(UserId);
            if (twoFactorAuthentication.HasErrors) return HasErrors(twoFactorAuthentication, nameof(GenerateRecoveryCodes), ManageControllerStrings.CannotGenerateRecoveryCodes2FaCheck);

            if (!twoFactorAuthentication.IsEnabled)
            {
                ModelState.AddModelError(string.Empty, ManageControllerStrings.CannotGenerateRecoveryCodesNotEnabled);
                return View(nameof(GenerateRecoveryCodes));
            }

            return null;
        }

        private IActionResult HasErrors(ManageBaseModel model, string viewName, string modelSateMessage)
        {
            LogErrors(model);
            ModelState.AddModelError(string.Empty, modelSateMessage);
            return View(viewName);
        }

        private void LogErrors(ManageBaseModel model)
        {
            if (model is null || !model.HasErrors) return;
            foreach (var error in model.Errors)
            {
                _logger.LogError("Error: userId {UserId} code {ErrorCode} message {ErrorMessage}", model.UserId, error.Code, error.Description);
            }
        }
    }
}
