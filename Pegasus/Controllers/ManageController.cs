using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Pegasus.Library.Api;
using Pegasus.Library.Models.Manage;
using Pegasus.Services;


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

        [Route(nameof(ChangePassword))]
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var userId = User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;

            var hasPasswordModel = await _manageEndpoint.HasPasswordAsync(new HasPasswordModel {UserId = userId});
            if (hasPasswordModel.UserNotFound)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
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

            var userId = User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
            model.UserId = userId;

            var changePasswordResult = await _manageEndpoint.ChangePasswordAsync(model);
            if (changePasswordResult.UserNotFound)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(userId);


            _logger.LogInformation("User with UserId {userId} changed their password successfully.", userId);
            model = new ChangePasswordModel
            {
                StatusMessage = "Your password has been changed."
            };

            return View(model);
        }

        [Route(nameof(SetPassword))]
        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var userId = User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;

            var hasPasswordModel = await _manageEndpoint.HasPasswordAsync(new HasPasswordModel {UserId = userId});
            if (hasPasswordModel.UserNotFound)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            if (hasPasswordModel.HasPassword)
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

            var userId = User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
            model.UserId = userId;
            var addPasswordResult = await _manageEndpoint.AddPasswordAsync(model);

            if (addPasswordResult.UserNotFound)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

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

        [Route(nameof(Index))]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
            var model = await _manageEndpoint.GetUserDetails(userId);
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
            if (model.Errors.Count > 0)
            {
                var statusMessage = new StringBuilder("Details were not updated.");
                foreach (var error in model.Errors)
                {
                    statusMessage.Append($" - {error}");
                }
                model.StatusMessage = statusMessage.ToString();
                return View(model);
            }

            model.StatusMessage = "User details were updated.";
            return View(model);
        }

        [Route(nameof(TwoFactorAuthentication))]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var model = await _manageEndpoint.TwoFactorAuthentication(User.Identity.Name);
            return View(model);
        }

        [Route(nameof(TwoFactorAuthentication))]
        [HttpPost]
        public async Task<IActionResult> TwoFactorAuthentication(TwoFactorAuthenticationModel model)
        {
            //TODO This is unlikely to work across the api
            // signInManager probably has access to the current browser instance
            //await _signInManager.ForgetTwoFactorClientAsync();
            await _manageEndpoint.ForgetTwoFactorClientAsync(User.Identity.Name);
            model.StatusMessage = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
            model.IsMachineRemembered = false;
            return View(model);
        }

        [Route(nameof(EnableAuthenticator))]
        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var authenticatorKeyModel =  await _manageEndpoint.LoadSharedKeyAndQrCodeUriAsync(User.Identity.Name);
            var model = new EnableAuthenticatorModel
            {
                SharedKey = authenticatorKeyModel.SharedKey, 
                AuthenticatorUri = authenticatorKeyModel.AuthenticatorUri
            };

            return View(model);
        }

        [Route(nameof(EnableAuthenticator))]
        [HttpPost]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorModel model)
        {
            var userId = User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!ModelState.IsValid)
            {
                var authenticatorKey =  await _manageEndpoint.LoadSharedKeyAndQrCodeUriAsync(User.Identity.Name);
                model.SharedKey = authenticatorKey.SharedKey;
                model.AuthenticatorUri = authenticatorKey.AuthenticatorUri;
                return View(model);
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var verifyTwoFactorTokenModel = new VerifyTwoFactorTokenModel()
            {
                Email = User.Identity.Name,
                VerificationCode = verificationCode
            };

            var response = await _manageEndpoint.VerifyTwoFactorTokenAsync(verifyTwoFactorTokenModel);
            if (!response.Is2FaTokenValid)
            {
                ModelState.AddModelError("Input.Code", "Verification code is invalid.");
                var authenticatorKey =  await _manageEndpoint.LoadSharedKeyAndQrCodeUriAsync(User.Identity.Name);
                model.SharedKey = authenticatorKey.SharedKey;
                model.AuthenticatorUri = authenticatorKey.AuthenticatorUri;
                return View(model);
            }

            await _signInManager.DoTwoFactorSignInAsync(userId, false);

            var setTwoFactorEnabledModel = new SetTwoFactorEnabledModel
            {
                Email = User.Identity.Name,
                Enabled = true
            };
            var set2FaEnabled = await _manageEndpoint.SetTwoFactorEnabledAsync(setTwoFactorEnabledModel);
            _logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", set2FaEnabled.UserId);
            model.StatusMessage = "Your authenticator app has been verified.";

            var recoveryCodeStatus = new RecoveryCodeStatusModel
            {
                Email = User.Identity.Name
            };
            recoveryCodeStatus = await _manageEndpoint.CheckRecoveryCodesStatus(recoveryCodeStatus);
            if (recoveryCodeStatus.NeededReset)
            {
                var showRecoveryCodesModel = new ShowRecoveryCodesModel()
                {
                    RecoveryCodes = recoveryCodeStatus.RecoveryCodes.ToArray()
                };
                return RedirectToAction("ShowRecoveryCodes", showRecoveryCodesModel);
            }

            return RedirectToAction("TwoFactorAuthentication");
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
            model.Email = User.Identity.Name;
            model = await _manageEndpoint.ResetAuthenticatorAsync(model);
    
            _logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", model.UserId);
            var enableAuthenticatorModel = new EnableAuthenticatorModel
            {
                StatusMessage =
                    "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key."
            };


            return RedirectToAction("EnableAuthenticator", "Manage", enableAuthenticatorModel);
        }

        [Route(nameof(Disable2Fa))]
        [HttpGet]
        public async Task<IActionResult> Disable2Fa()
        {
            var response = await _manageEndpoint.GetTwoFactorEnabledAsync(User.Identity.Name);
            if (!response.Enabled)
            {
                throw new InvalidOperationException($"Cannot disable 2FA for user with ID '{response.UserId}' as it's not currently enabled.");
            }

            return View();
        }

        [Route(nameof(Disable2Fa))]
        [HttpPost]
        public async Task<IActionResult> Disable2Fa(Disable2FaModel model)
        {
            var setTwoFactorEnabled = new SetTwoFactorEnabledModel
            {
                Email = User.Identity.Name
            };
            var disable2FaResult = await _manageEndpoint.SetTwoFactorEnabledAsync(setTwoFactorEnabled);

            if (!disable2FaResult.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred disabling 2FA for user with ID '{disable2FaResult.UserId}'.");
            }

            _logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", disable2FaResult.UserId);
            var twoFactorAuthenticationModel = new TwoFactorAuthenticationModel
            {
                StatusMessage = "2fa has been disabled. You can reenable 2fa when you setup an authenticator app"
            };
            return RedirectToAction("TwoFactorAuthentication", "Manage", twoFactorAuthenticationModel);
        }

        [Route(nameof(GenerateRecoveryCodes))]
        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var response = await _manageEndpoint.GetTwoFactorEnabledAsync(User.Identity.Name);
            if (!response.Enabled)
            {
                throw new InvalidOperationException($"Cannot generate recovery codes for user with ID '{response.UserId}' because they do not have 2FA enabled.");
            }

            return View();
        }

        [Route(nameof(GenerateRecoveryCodes))]
        [HttpPost]
        public async Task<IActionResult> GenerateRecoveryCodes(GenerateRecoveryCodesModel model)
        {
            var response = await _manageEndpoint.GetTwoFactorEnabledAsync(User.Identity.Name);
            if (!response.Enabled)
            {
                throw new InvalidOperationException($"Cannot generate recovery codes for user with ID '{response.UserId}' as they do not have 2FA enabled.");
            }

            model = await _manageEndpoint.GenerateNewRecoveryCodesAsync(User.Identity.Name);
            var showRecoveryCodes = new ShowRecoveryCodesModel
            {
                RecoveryCodes = model.RecoveryCodes.ToArray()
            };

            _logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", response.UserId);

            showRecoveryCodes.StatusMessage = "You have generated new recovery codes.";
            return RedirectToAction("ShowRecoveryCodes", showRecoveryCodes);
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
    }
}
