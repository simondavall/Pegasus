using System.Linq;
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
            if (hasPasswordModel.HasErrors) return HasErrors(hasPasswordModel, nameof(ChangePassword), ManageControllerStrings.CannotChangePassword);

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
                LogErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(UserId);

            _logger.LogInformation("User with UserId {UserId} changed their password successfully", UserId);
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
            var model = new Disable2FaModel();
            var (enabled, actionResult) = await CheckTwoFactorEnabledAsync(model);
            if (!enabled)
            {
                return actionResult;
            }

            return View(model);
        }

        [Route(nameof(Disable2Fa))]
        [HttpPost]
        public async Task<IActionResult> Disable2Fa(Disable2FaModel model)
        {
            var (enabled, actionResult) = await CheckTwoFactorEnabledAsync(model);
            if (!enabled)
                return actionResult;

            var setTwoFactorEnabled = new SetTwoFactorEnabledModel
            {
                UserId = UserId,
                SetEnabled = false
            };

            var disable2FaResult = await _manageEndpoint.SetTwoFactorEnabledAsync(setTwoFactorEnabled);
            if (!disable2FaResult.Succeeded)
            {
                _logger.LogError("Failed to disable 2fa for User with ID '{UserId}'", UserId);
                LogErrors(disable2FaResult);
                model.StatusMessage = ManageControllerStrings.FailedToDisable2Fa;
                return View(model);
            }
            
            _logger.LogTrace("User with ID '{UserId}' has disabled 2fa", UserId);
            return RedirectToAction("TwoFactorAuthentication", "Manage");
        }

        [Route(nameof(EnableAuthenticator))]
        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var enableAuthenticatorModel = await GetEnableAuthenticatorModel(UserId);
            if (enableAuthenticatorModel.HasErrors)
            {
                ModelState.AddModelError(string.Empty,  ManageControllerStrings.CannotEnableAuthenticator);
                LogErrors(enableAuthenticatorModel);
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

            var (verified, actionResult) = await VerifyTwoFactorTokenAsync(model);
            if (!verified) 
                return actionResult;

            await _signInManager.DoTwoFactorSignInAsync(UserId, false);

            var (enabled, actionResult1) = await SetTwoFactorEnabledAsync(model);
            if (!enabled) 
                return actionResult1;

            _logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app", UserId);
            model.StatusMessage = ManageControllerStrings.AuthenticatorAppVerified;

            var (isUpdated, actionResult2) = await CheckRecoveryCodesStatusAsync(model);
            if (isUpdated)
            {
                return actionResult2;
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

            _logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes", UserId);

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
                model.Username = User.Identity?.Name;
                return View(model);
            }

            model = await _manageEndpoint.SetUserDetails(model);
            if (model.HasErrors) return HasErrors(model, nameof(Index), ManageControllerStrings.CannotUpdateUserDetails, model);

            model.StatusMessage = ManageControllerStrings.UserDetailsUpdated;
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
            if (model.HasErrors) return HasErrors(model, nameof(ResetAuthenticator), ManageControllerStrings.FailedToResetAuthenticator, model);

            _logger.LogInformation("User with ID '{UserId}' has reset their authentication app key", model.UserId);
            var enableAuthenticatorModel = new EnableAuthenticatorModel
            {
                StatusMessage = ManageControllerStrings.AuthenticatorResetSuccess
            };

            return RedirectToAction(nameof(EnableAuthenticator), enableAuthenticatorModel);
        }
                
        [Route(nameof(SetPassword))]
        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var model = await _manageEndpoint.HasPasswordAsync(UserId);
            if (model.HasErrors) return HasErrors(model, nameof(SetPassword));

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
            if (addPasswordResult.HasErrors) return HasErrors(addPasswordResult, nameof(SetPassword), null, model);

            if (!addPasswordResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, ManageControllerStrings.FailedToSetPassword);
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(model.UserId);

            model = new SetPasswordModel {StatusMessage = ManageControllerStrings.PasswordSetSuccess};
            return View(model);
        }

        [Route(nameof(ShowRecoveryCodes))]
        [HttpGet]
        public IActionResult ShowRecoveryCodes(ShowRecoveryCodesModel model)
        {
            if (model.RecoveryCodes == null || model.RecoveryCodes.Length == 0)
            {
                return RedirectToAction(nameof(TwoFactorAuthentication));
            }

            return View(model);
        }

        [Route(nameof(TwoFactorAuthentication))]
        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication(TwoFactorAuthenticationModel model)
        {
            if (string.IsNullOrWhiteSpace(model?.UserId))
                model = await _manageEndpoint.TwoFactorAuthentication(UserId);

            model.IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(UserId);
            return View(model);
        }

        [Route(nameof(ForgetThisBrowser))]
        [HttpGet]
        public async Task<IActionResult> ForgetThisBrowser()
        {
            await _signInManager.ForgetTwoFactorClientAsync();
            var model = await _manageEndpoint.TwoFactorAuthentication(UserId);
            model.StatusMessage = ManageControllerStrings.CurrentBrowserForgotten;
            return RedirectToAction(nameof(TwoFactorAuthentication), model);
        }


                
        private async Task<(bool isUpdated, IActionResult actionResult)> CheckRecoveryCodesStatusAsync(EnableAuthenticatorModel model)
        {
            var recoveryCodeStatus = new RecoveryCodeStatusModel {UserId = UserId};
            recoveryCodeStatus = await _manageEndpoint.CheckRecoveryCodesStatusAsync(recoveryCodeStatus);
            if (recoveryCodeStatus.HasErrors)
            {
                var result = HasErrors(recoveryCodeStatus, nameof(EnableAuthenticator), ManageControllerStrings.CannotEnableAuthenticatorRecoveryCodes, model);
                return (true, result);
            }
            
            if (recoveryCodeStatus.IsUpdated)
            {
                var showRecoveryCodesModel = new ShowRecoveryCodesModel()
                {
                    StatusMessage = model.StatusMessage,
                    RecoveryCodes = recoveryCodeStatus.RecoveryCodes?.ToArray()
                };
                return (true, RedirectToAction("ShowRecoveryCodes", showRecoveryCodesModel));
            }
            return (false, null);
        }

        //TODO see if we can combine these two calls to GetTwoFactorEnabledAsync
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
        //TODO see if we can combine these two calls to GetTwoFactorEnabledAsync
        private async Task<(bool enabled, IActionResult actionResult)> CheckTwoFactorEnabledAsync(ManageBaseModel model)
        {
            var twoFactorAuthentication = await _manageEndpoint.GetTwoFactorEnabledAsync(UserId);
            if (!twoFactorAuthentication.IsEnabled)
            {
                model.StatusMessage = ManageControllerStrings.TwoFactorNotEnabled;
                return (false, View(model));
            }

            return (true, null);
        }

        private IActionResult HasErrors(ManageBaseModel model, string viewName, string modelSateMessage = null, ManageBaseModel returnModel = null)
        {
            if (model is { HasErrors: true })
            {
                if (!string.IsNullOrWhiteSpace(modelSateMessage))
                {
                    ModelState.AddModelError(string.Empty, modelSateMessage);
                }

                LogErrors(model);
            }

            return View(viewName, returnModel);
        }

        private async Task<EnableAuthenticatorModel> GetEnableAuthenticatorModel(string userId)
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

        private void LogErrors(ManageBaseModel model)
        {
            if (model is null || !model.HasErrors) return;
            foreach (var error in model.Errors)
            {
                _logger.LogError("Error: userId {UserId} code {ErrorCode} message {ErrorMessage}", model.UserId, error.Code, error.Description);
                ModelState.AddModelError(error.Code, error.Description);
            }
        }

        private async Task<(bool enabled, IActionResult actionResult)> SetTwoFactorEnabledAsync(EnableAuthenticatorModel model)
        {
            var setTwoFactorEnabledModel = new SetTwoFactorEnabledModel
            {
                UserId = UserId,
                SetEnabled = true
            };
            var set2FaEnabled = await _manageEndpoint.SetTwoFactorEnabledAsync(setTwoFactorEnabledModel);
            if (set2FaEnabled.HasErrors)
            {
                var result = HasErrors(set2FaEnabled, nameof(EnableAuthenticator), ManageControllerStrings.CannotEnableAuthenticatorSet2FaEnabled, model);
                return (false, result);
            }
            
            return (true, null);
        }

        private async Task<(bool verified, IActionResult actionResult)> VerifyTwoFactorTokenAsync(EnableAuthenticatorModel model)
        {
            var verifyTwoFactorTokenModel = new VerifyTwoFactorTokenModel
            {
                UserId = UserId,
                VerificationCode = model.Code?.Replace(" ", string.Empty).Replace("-", string.Empty)
            };

            verifyTwoFactorTokenModel = await _manageEndpoint.VerifyTwoFactorTokenAsync(verifyTwoFactorTokenModel);
            if (verifyTwoFactorTokenModel.HasErrors)
            {
                var result1 = HasErrors(verifyTwoFactorTokenModel, nameof(EnableAuthenticator), ManageControllerStrings.CannotEnableAuthenticatorVerifyToken, model);
                return (false, result1);
            }
            
            if (!verifyTwoFactorTokenModel.IsTokenValid)
            {
                ModelState.AddModelError(string.Empty, ManageControllerStrings.CannotEnableAuthenticatorTokenInvalid);
                var result = await EnableAuthenticator();
                return (false, result);
            }

            return (true, null);
        }

    }
}
