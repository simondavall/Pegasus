using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Controllers
{
    [Authorize(Roles = "PegasusUser")]
    [Route("api/Account/[controller]")]
    [ApiController]
    public class ManageController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUsersData _usersData;
        private readonly UrlEncoder _urlEncoder;
        private readonly IConfiguration _configuration;
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public ManageController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IUsersData usersData, 
            UrlEncoder urlEncoder, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _usersData = usersData;
            _urlEncoder = urlEncoder;
            _configuration = configuration;
        }

        [Route("AddPassword")]
        [HttpPost]
        public async Task<SetPasswordModel> AddPassword(SetPasswordModel model)
        {
            var user = await GetUser(model.UserId, model);
            if (user == null)
                return model;

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            model.Succeeded = addPasswordResult.Succeeded;
            model.Errors = addPasswordResult.Errors.ToList();
            return model;
        }
        
        [Route("ChangePassword")]
        [HttpPost]
        public async Task<ChangePasswordModel> ChangePassword(ChangePasswordModel model)
        {
            var user = await GetUser(model.UserId, model);
            if (user == null)
                return model;

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            model.Succeeded = changePasswordResult.Succeeded;
            model.Errors = changePasswordResult.Errors.ToList();
            return model;
        }

        [Route("CheckRecoveryCodesStatus")]
        [HttpPost]
        public async Task<RecoveryCodeStatusModel> CheckRecoveryCodesStatus(RecoveryCodeStatusModel model)
        {
            var user = await GetUser(model.UserId, model);
            if (user == null)
                return model;

            if (await _userManager.CountRecoveryCodesAsync(user) == 0)
            {
                model.IsUpdated = true;
                model.RecoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            }

            return model;
        }

        [Route("GenerateNewRecoveryCodes/{userId}")]
        [HttpGet]
        public async Task<GenerateRecoveryCodesModel> GenerateNewRecoveryCodes(string userId)
        {
            var model = new GenerateRecoveryCodesModel();
            var user = await GetUser(userId, model);
            if (user == null)
                return model;

            model.RecoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            return model;
        }

        [Route("GetTwoFactorEnabled/{userId}")]
        [HttpGet]
        public async Task<GetTwoFactorEnabledModel> GetTwoFactorEnabled(string userId)
        {
            var model = new GetTwoFactorEnabledModel();
            var user = await GetUser(userId, model);
            if (user == null)
                return model;

            model.IsEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            return model;
        }

        [Route("GetUserDetails/{userId}")]
        [HttpGet]
        public async Task<UserDetailsModel> GetUserDetails(string userId)
        {
            var model = new UserDetailsModel();
            var user = await GetUser(userId, model);
            if (user == null)
                return model;

            model.UserId = userId;
            model.Username = user.UserName;
            model.PhoneNumber = await _userManager.GetPhoneNumberAsync(user);

            var customData = await _usersData.GetUser(userId);
            model.DisplayName = customData?.DisplayName;

            return model;
        }

        [Route("HasPassword/{userId}")]
        [HttpGet]
        public async Task<HasPasswordModel> HasPassword(string userId)
        {
            var model = new HasPasswordModel();
            var user = await GetUser(userId, model);
            if (user == null)
                return model;

            model.HasPassword = await _userManager.HasPasswordAsync(user);
            return model;
        }

        [Route("LoadSharedKeyAndQrCodeUri/{userId}")]
        [HttpGet]
        public async Task<AuthenticatorKeyModel> LoadSharedKeyAndQrCodeUri(string userId)
        {
            var model = new AuthenticatorKeyModel();
            var user = await GetUser(userId, model);
            if (user == null)
                return model;

            return await LoadSharedKeyAndQrCodeUriAsync(user);
        }

        [Route("ResetAuthenticator")]
        [HttpPost]
        public async Task<ResetAuthenticatorModel> ResetAuthenticator(ResetAuthenticatorModel model)
        {
            var user = await GetUser(model.UserId, model);
            if (user == null)
                return model;

            var set2FaResult =  await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!set2FaResult.Succeeded)
            {
                model.Errors = set2FaResult.Errors.ToList();
                return model;
            }

            var resetResult = await _userManager.ResetAuthenticatorKeyAsync(user);
            if (!resetResult.Succeeded)
            {
                model.Errors = resetResult.Errors.ToList();
            }

            return model;
        }

        [Route("SetTwoFactorEnabled")]
        [HttpPost]
        public async Task<SetTwoFactorEnabledModel> SetTwoFactorEnabled(SetTwoFactorEnabledModel model)
        {
            var user = await GetUser(model.UserId, model);
            if (user == null)
                return model;

            var set2FaEnabledResult = await _userManager.SetTwoFactorEnabledAsync(user, model.SetEnabled);
            model.Succeeded = set2FaEnabledResult.Succeeded;
            model.Errors = set2FaEnabledResult.Errors.ToList();

            return model;
        }

        [Route("SetUserDetails")]
        [HttpPost]
        public async Task<UserDetailsModel> SetUserDetails(UserDetailsModel model)
        {
            var user = await GetUser(model.UserId, model);
            if (user == null)
                return model;

            model.Username = user.UserName;
            
            var setPhoneNumber = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
            if (!setPhoneNumber.Succeeded)
            {
                model.Errors = setPhoneNumber.Errors.ToList();
            }

            try
            {
                await _usersData.UpdateUser(new UserModel {Id = model.UserId, DisplayName = model.DisplayName});
            }
            catch (Exception e)
            {
                model.Errors.Add(Error( $"Error when saving custom User Settings. Message: {e.Message}"));
            }

            return model;
        }

        [AllowAnonymous]
        [Route("TwoFactorAuthentication/{userId}")]
        [HttpGet]
        public async Task<TwoFactorAuthenticationModel> TwoFactorAuthentication(string userId)
        {
            var model = new TwoFactorAuthenticationModel();
            var user = await GetUser(userId, model);
            if (user == null)
                return model;

            model.HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null;
            model.Is2FaEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            model.IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user);
            model.RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user);

            return model;
        }

        [Route("VerifyTwoFactorToken")]
        [HttpPost]
        public async Task<VerifyTwoFactorTokenModel> VerifyTwoFactorToken(VerifyTwoFactorTokenModel model)
        {
            var user = await GetUser(model.UserId, model);
            if (user == null)
                return model;

            model.IsTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, model.VerificationCode);
            return model;
        }

        private async Task<IdentityUser> GetUser<T>(string userId, T model) where T : ManageBaseModel
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                model.Errors = UserNotFoundError(userId);
            }

            return user;
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

        private static string FormatKey(string unformattedKey)
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
                _urlEncoder.Encode(_configuration["ProjectName"]),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        private static IdentityError Error(string message)
        {
            return new IdentityError {Code = string.Empty, Description = message};
        }

        private static List<IdentityError> UserNotFoundError(string userId)
        {
            return new List<IdentityError> {Error($"Unable to load user with userId '{userId}'.")};
    }
    }
}
