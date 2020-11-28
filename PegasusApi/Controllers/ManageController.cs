using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Controllers
{
    //TODO Add PegasusUser authorization
    [Route("api/Account/[controller]")]
    [ApiController]
    public class ManageController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UrlEncoder _urlEncoder;
        private readonly ILogger<ManageController> _logger;
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public ManageController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, 
            UrlEncoder urlEncoder, ILogger<ManageController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _urlEncoder = urlEncoder;
            _logger = logger;
        }
        
        [Route("GetUserDetails/{userId}")]
        [HttpGet]
        public async Task<UserDetailsModel> GetUserDetails(string userId)
        {
            var model = new UserDetailsModel
            {
                UserId = userId
            };
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                model.StatusMessage = $"Unable to load user with userId '{model.UserId}'.";
                return model;
            }
            model.UserId = user.Id;
            model.Username = user.UserName;
            model.PhoneNumber = await _userManager.GetPhoneNumberAsync(user);
            return model;
        }

        [Route("SetUserDetails")]
        [HttpPost]
        public async Task<UserDetailsModel> SetUserDetails(UserDetailsModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                model.StatusMessage = $"Unable to load user with userId '{model.UserId}'.";
                return model;
            }

            model.Username = user.UserName;
            var phoneValidator = new PhoneAttribute();
            if (!phoneValidator.IsValid(model.PhoneNumber))
            {
                model.Errors.Add("Invalid phone number format.");
            }

            if (model.Errors.Count != 0)
            {
                return model;
            }

            try
            {
                await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
            }
            catch (Exception e)
            {
                model.Errors.Add($"Error when saving User Settings. Message: {e.Message}, Error:{e}");
            }

            return model;
        }

        [AllowAnonymous]
        [Route("TwoFactorAuthentication/{email}")]
        [HttpGet]
        public async Task<TwoFactorAuthenticationModel> TwoFactorAuthentication(string email)
        {
            var model = new TwoFactorAuthenticationModel();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                model.StatusMessage = $"Unable to load user with email '{email}'.";
                return model;
            }

            model.HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null;
            model.Is2FaEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            model.IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user);
            model.RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user);

            return model;
        }

        [Route("LoadSharedKeyAndQrCodeUri/{email}")]
        [HttpGet]
        public async Task<AuthenticatorKeyModel> LoadSharedKeyAndQrCodeUri(string email)
        {
            var model = new AuthenticatorKeyModel();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                model.StatusMessage = $"Unable to load user with email '{email}'.";
                return model;
            }

            return await LoadSharedKeyAndQrCodeUriAsync(user);
        }

        [Route("VerifyTwoFactorToken")]
        [HttpPost]
        public async Task<VerifyTwoFactorTokenModel> VerifyTwoFactorToken(VerifyTwoFactorTokenModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                model.StatusMessage = $"Unable to load user with email '{model.Email}'.";
                return model;
            }
            model.Is2FaTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, model.VerificationCode);
            return model;
        }

        [Route("SetTwoFactorEnabled")]
        [HttpPost]
        public async Task<SetTwoFactorEnabledModel> SetTwoFactorEnabled(SetTwoFactorEnabledModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    model.StatusMessage = $"Unable to load user with email '{model.Email}'.";
                    model.Succeeded = false;
                    model.Enabled = false;
                    return model;
                }
                model.UserId = user.Id;
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                model.Succeeded = true;
                model.Enabled = user.TwoFactorEnabled;
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to enable Two Factor Authentication for user {model.Email}. Reason:{e.Message}");
                model.Succeeded = false;
                model.StatusMessage = $"Failed to enable Two Factor Authentication. Reason:{e.Message}";
            }

            return model;
        }

        [Route("GetTwoFactorEnabled/{email}")]
        [HttpGet]
        public async Task<GetTwoFactorEnabledModel> GetTwoFactorEnabled(string email)
        {
            var model = new GetTwoFactorEnabledModel
            {
                Email = email
            };
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                model.StatusMessage = $"Unable to load user with email '{model.Email}'.";
                return model;
            }
            model.UserId = user.Id;
            model.Enabled = await _userManager.GetTwoFactorEnabledAsync(user);
            return model;
        }

        [Route("GenerateNewRecoveryCodes/{email}")]
        [HttpGet]
        public async Task<GenerateRecoveryCodesModel> GenerateNewRecoveryCodes(string email)
        {
            var model = new GenerateRecoveryCodesModel();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                model.StatusMessage = $"Unable to load user with email '{email}'.";
                return model;
            }
            model.RecoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            return model;
        }

        [Route("CheckRecoveryCodesStatus")]
        [HttpPost]
        public async Task<RecoveryCodeStatusModel> CheckRecoveryCodesStatus(RecoveryCodeStatusModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                model.StatusMessage = $"Unable to load user with email '{model.Email}'.";
                return model;
            }

            if (await _userManager.CountRecoveryCodesAsync(user) == 0)
            {
                model.NeededReset = true;
                model.RecoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            }

            return model;
        }

        [Route("ResetAuthenticator")]
        [HttpPost]
        public async Task<ResetAuthenticatorModel> ResetAuthenticator(ResetAuthenticatorModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                model.StatusMessage = $"Unable to load user with email '{model.Email}'.";
                return model;
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);
            
            model.StatusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";
            return model;
        }

        private async Task<AuthenticatorKeyModel> LoadSharedKeyAndQrCodeUriAsync(IdentityUser user)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var model = new AuthenticatorKeyModel();
            model.SharedKey = FormatKey(unformattedKey);

            var email = await _userManager.GetEmailAsync(user);
            model.AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);

            return model;
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("AuthenticationRazorWith2fa"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }
    }
}
