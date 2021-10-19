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
using Microsoft.Extensions.Logging;
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
        private readonly IUsersData _usersData;
        private readonly UrlEncoder _urlEncoder;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ManageController> _logger;
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public ManageController(UserManager<IdentityUser> userManager, IUsersData usersData, 
            UrlEncoder urlEncoder, IConfiguration configuration, ILogger<ManageController> logger)
        {
            _userManager = userManager;
            _usersData = usersData;
            _urlEncoder = urlEncoder;
            _configuration = configuration;
            _logger = logger;
        }

        [Route("AddPassword")]
        [HttpPost]
        public async Task<SetPasswordModel> AddPassword(SetPasswordModel model)
        {
            var (hasErrors, returnModel, user) = await ValidateModelAndGetUser(model);
            if (hasErrors)
            {
                return returnModel;
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (addPasswordResult.Succeeded)
            {
                model.Succeeded = true;
                return model;
            }

            model.Errors = addPasswordResult.Errors.ToList();
            LogErrors(model , "Failed to add password");

            return model;
        }
        
        [Route("ChangePassword")]
        [HttpPost]
        public async Task<ChangePasswordModel> ChangePassword(ChangePasswordModel model)
        {
            var (hasErrors, returnModel, user) = await ValidateModelAndGetUser(model);
            if (hasErrors)
            {
                return returnModel;
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (changePasswordResult.Succeeded)
            {
                model.Succeeded = true;
                return model;
            }

            model.Errors = changePasswordResult.Errors.ToList();
            LogErrors(model , "Failed to add password");

            return model;
        }

        [Route("CheckRecoveryCodesStatus")]
        [HttpPost]
        public async Task<RecoveryCodeStatusModel> CheckRecoveryCodesStatus(RecoveryCodeStatusModel model)
        {
            var (hasErrors, returnModel, user) = await ValidateModelAndGetUser(model);
            if (hasErrors)
            {
                return returnModel;
            }

            if (await _userManager.CountRecoveryCodesAsync(user) != 0)
            {
                return model;
            }
            
            const int numberOfCodesToCreate = 10;
            model.RecoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, numberOfCodesToCreate);
            if (model.RecoveryCodes is null || !model.RecoveryCodes.Any())
            {
                _logger.LogError("Failed to generate recovery codes for userId {UserId}", model.UserId);
                model.Errors.Add(new IdentityError(){Code = string.Empty, Description = "Failed to generate recovery codes"});
            }
            else
            {
                model.IsUpdated = true;
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
            
            const int numberOfCodesToCreate = 10;
            model.RecoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, numberOfCodesToCreate);
            if (model.RecoveryCodes is null || !model.RecoveryCodes.Any())
            {
                _logger.LogError("Failed to generate recovery codes for userId {UserId}", model.UserId);
                model.Errors.Add(new IdentityError {Code = string.Empty, Description = "Failed to generate recovery codes"});
            }
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

            var userModel = await _usersData.GetUser(userId);
            model.DisplayName = userModel?.DisplayName;

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

            return await GetSharedKeyAndQrCodeUriAsync(user);
        }

        [Route("ResetAuthenticator")]
        [HttpPost]
        public async Task<ResetAuthenticatorModel> ResetAuthenticator(ResetAuthenticatorModel model)
        {
            var (hasErrors, returnModel, user) = await ValidateModelAndGetUser(model);
            if (hasErrors)
            {
                return returnModel;
            }
            
            var set2FaResult =  await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!set2FaResult.Succeeded)
            {
                model.Errors = set2FaResult.Errors.ToList();
                LogErrors(model, "Failed to enable two factor authentication.");
                return model;
            }

            var resetResult = await _userManager.ResetAuthenticatorKeyAsync(user);
            if (!resetResult.Succeeded)
            {
                model.Errors = resetResult.Errors.ToList();
                LogErrors(model, "Failed to reset authenticator key.");
            }

            return model;
        }

        [Route("SetTwoFactorEnabled")]
        [HttpPost]
        public async Task<SetTwoFactorEnabledModel> SetTwoFactorEnabled(SetTwoFactorEnabledModel model)
        {
            var (hasErrors, returnModel, user) = await ValidateModelAndGetUser(model);
            if (hasErrors)
            {
                return returnModel;
            }

            var set2FaEnabledResult = await _userManager.SetTwoFactorEnabledAsync(user, model.SetEnabled);
            if (set2FaEnabledResult.Succeeded)
            {
                model.Succeeded = true;
                return model;
            }

            model.Errors = set2FaEnabledResult.Errors.ToList();
            LogErrors(model , "Failed to set two factor authentication");

            return model;
        }

        [Route("SetUserDetails")]
        [HttpPost]
        public async Task<UserDetailsModel> SetUserDetails(UserDetailsModel model)
        {
            var (hasErrors, returnModel, user) = await ValidateModelAndGetUser(model);
            if (hasErrors)
            {
                return returnModel;
            }

            //TODO Look at being able to change username(email)
            model.Username = user.UserName;
            
            var setPhoneNumber = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
            if (!setPhoneNumber.Succeeded)
            {
                model.Errors = setPhoneNumber.Errors.ToList();
                LogErrors(model , "Failed to set phone number");
            }

            try
            {
                await _usersData.UpdateUser(new UserModel {Id = model.UserId, DisplayName = model.DisplayName});
            }
            catch (Exception e)
            {
                model.Errors.Add(Error( $"Error when saving user details. Message: {e.Message}"));
                _logger.LogError(e, "Failed to update user details for userId {UserId}", model.UserId);
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
            model.RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user);
            model.UserId = userId;

            return model;
        }

        [Route("VerifyTwoFactorToken")]
        [HttpPost]
        public async Task<VerifyTwoFactorTokenModel> VerifyTwoFactorToken(VerifyTwoFactorTokenModel model)
        {
            var (hasErrors, returnModel, user) = await ValidateModelAndGetUser(model);
            if (hasErrors)
            {
                return returnModel;
            }
            
            model.IsTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, model.VerificationCode);
            return model;
        }

        private static IdentityError Error(string message)
        {
            return new IdentityError {Code = string.Empty, Description = message};
        }

        private static string FormatKeyByAddingSpaceEveryFourChars(string unformattedKey)
        {
            const int numberOfChars = 4;
            var result = new StringBuilder();
            var currentPosition = 0;
            while (currentPosition + numberOfChars < unformattedKey.Length)
            {
                result
                    .Append(unformattedKey.Substring(currentPosition, numberOfChars))
                    .Append(" ");
                
                currentPosition += numberOfChars;
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
        
        private async Task<AuthenticatorKeyModel> GetSharedKeyAndQrCodeUriAsync(IdentityUser user)
        {
            var model = new AuthenticatorKeyModel();
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                var resetResult = await _userManager.ResetAuthenticatorKeyAsync(user);
                if (!resetResult.Succeeded)
                {
                    model.Errors = resetResult.Errors.ToList();
                    LogErrors(model, "Failed to reset Authenticator Key");
                    return model;
                }
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }
            if (string.IsNullOrEmpty(unformattedKey))
            {
                _logger.LogError("Unable to retrieve Authenticator Key for userId {UserId}", model.UserId);
                model.Errors.Add(new IdentityError(){Code=string.Empty, Description = "Unable to retrieve Authenticator Key"});
                return model;
            }
            
            model.SharedKey = FormatKeyByAddingSpaceEveryFourChars(unformattedKey);

            var email = await _userManager.GetEmailAsync(user);
            model.AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);

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

        private bool IsModelNull<T>(ref T model) where T : ManageBaseModel, new()
        {
            if (model is { })
            {
                return false;
            }
            
            model = new T();
            model.Errors.Add(new IdentityError
            {
                Code = string.Empty, 
                Description = "Null argument passed to api"
            });
            return true;
        }

        private void LogErrors(ManageBaseModel model, string errorMessage)
        {
            foreach (var identityError in model.Errors)
            {
                _logger.LogError("{ErrorMessage} for userId {UserId}. Error - {Error}", errorMessage, model.UserId, identityError.Description);
            }
        }
        
        private List<IdentityError> UserNotFoundError(string userId)
        {
            _logger.LogError("User not found for userId {UserId}", userId);
            return new List<IdentityError> {Error($"User not found with userId '{userId}'.")};
        }

        private async Task<(bool hasErrors, T model, IdentityUser user)> ValidateModelAndGetUser<T>(T model) where T : ManageBaseModel, new()
        {
            if (IsModelNull(ref model))
            {
                return (true, model, null);
            }
            
            var user = await GetUser(model.UserId, model);
            if (user == null)
            {
                return (true, model, null);
            }
            
            return (false, model, user);
        }
    }
}
