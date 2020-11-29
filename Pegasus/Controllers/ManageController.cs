﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Pegasus.Library.Api;
using Pegasus.Library.JwtAuthentication;
using Pegasus.Library.Models.Manage;
using Pegasus.Services;

namespace Pegasus.Controllers
{
    [Authorize(Roles="PegasusUser")]
    [Route("Account/Manage")]
    public class ManageController : Controller
    {
        private readonly IManageEndpoint _manageEndpoint;
        private readonly IAuthenticationEndpoint _authenticationEndpoint;
        private readonly IApiHelper _apiHelper;
        private readonly IJwtTokenAccessor _tokenAccessor;
        private readonly ILogger<ManageController> _logger;

        public ManageController(IManageEndpoint manageEndpoint, IAuthenticationEndpoint authenticationEndpoint,
            IApiHelper apiHelper, IJwtTokenAccessor tokenAccessor, ILogger<ManageController> logger)
        {
            _manageEndpoint = manageEndpoint;
            _authenticationEndpoint = authenticationEndpoint;
            _apiHelper = apiHelper;
            _tokenAccessor = tokenAccessor;
            _logger = logger;
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

            //TODO This is repeated in the Login2fa controller action (see if extract method possible)
            // refresh sign in with 2fa
            var authenticatedUser = await _authenticationEndpoint.Authenticate2Fa(userId);
            var accessTokenResult = _tokenAccessor.GetAccessTokenWithClaimsPrincipal(authenticatedUser);

            //Need to re-sign in with 2fa
            await HttpContext.SignOutAsync();
            await HttpContext.SignInAsync(accessTokenResult.ClaimsPrincipal, accessTokenResult.AuthenticationProperties);

            _apiHelper.AddTokenToHeaders(accessTokenResult.AccessToken);

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
