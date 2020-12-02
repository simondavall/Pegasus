using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pegasus.Library.Models.Manage;

namespace Pegasus.Library.Api
{
    public interface IManageEndpoint
    {
        Task<SetPasswordModel> AddPasswordAsync(SetPasswordModel model);
        Task<ChangePasswordModel> ChangePasswordAsync(ChangePasswordModel model);
        Task<RecoveryCodeStatusModel> CheckRecoveryCodesStatus(RecoveryCodeStatusModel model);
        Task<GenerateRecoveryCodesModel> GenerateNewRecoveryCodesAsync(string userId);
        Task<GetTwoFactorEnabledModel> GetTwoFactorEnabledAsync(string userId);
        Task<UserDetailsModel> GetUserDetails(string userId);
        Task<HasPasswordModel> HasPasswordAsync(string userId);
        Task<AuthenticatorKeyModel> LoadSharedKeyAndQrCodeUriAsync(string userId);
        Task<ResetAuthenticatorModel> ResetAuthenticatorAsync(ResetAuthenticatorModel model);
        Task<SetTwoFactorEnabledModel> SetTwoFactorEnabledAsync(SetTwoFactorEnabledModel model);
        Task<UserDetailsModel> SetUserDetails(UserDetailsModel model);
        Task<TwoFactorAuthenticationModel> TwoFactorAuthentication(string userId);
        Task<VerifyTwoFactorTokenModel> VerifyTwoFactorTokenAsync(VerifyTwoFactorTokenModel model);
    }

    [Authorize(Roles = "PegasusUser")]
    public class ManageEndpoint : IManageEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public ManageEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<SetPasswordModel> AddPasswordAsync(SetPasswordModel model)
        {
            return await _apiHelper.PostAsync(model, "api/Account/Manage/AddPassword");
        }

        public async Task<ChangePasswordModel> ChangePasswordAsync(ChangePasswordModel model)
        {
            return await _apiHelper.PostAsync(model, "api/Account/Manage/ChangePassword");
        }

        public async Task<RecoveryCodeStatusModel> CheckRecoveryCodesStatus(RecoveryCodeStatusModel model)
        {
            return await _apiHelper.PostAsync(model, "api/Account/Manage/CheckRecoveryCodesStatus");
        }

        public async Task<GenerateRecoveryCodesModel> GenerateNewRecoveryCodesAsync(string userId)
        {
            return await _apiHelper.GetFromUri<GenerateRecoveryCodesModel>($"api/Account/Manage/GenerateNewRecoveryCodes/{userId}");
        }

        public async Task<GetTwoFactorEnabledModel> GetTwoFactorEnabledAsync(string userId)
        {
            return await _apiHelper.GetFromUri<GetTwoFactorEnabledModel>($"api/Account/Manage/GetTwoFactorEnabled/{userId}");
        }

        public async Task<UserDetailsModel> GetUserDetails(string userId)
        {
            return await _apiHelper.GetFromUri<UserDetailsModel>($"api/Account/Manage/GetUserDetails/{userId}");
        }

        public async Task<HasPasswordModel> HasPasswordAsync(string userId)
        {
            return await _apiHelper.GetFromUri<HasPasswordModel>($"api/Account/Manage/HasPassword/{userId}");
        }

        public async Task<AuthenticatorKeyModel> LoadSharedKeyAndQrCodeUriAsync(string userId)
        {
            return await _apiHelper.GetFromUri<AuthenticatorKeyModel>($"api/Account/Manage/LoadSharedKeyAndQrCodeUri/{userId}");
        }

        public async Task<ResetAuthenticatorModel> ResetAuthenticatorAsync(ResetAuthenticatorModel model)
        {
            return await _apiHelper.PostAsync(model, "api/Account/Manage/ResetAuthenticator");
        }
        
        public async Task<SetTwoFactorEnabledModel> SetTwoFactorEnabledAsync(SetTwoFactorEnabledModel model)
        {
            return await _apiHelper.PostAsync(model, "api/Account/Manage/SetTwoFactorEnabled");
        }

        public async Task<UserDetailsModel> SetUserDetails(UserDetailsModel model)
        {
            return await _apiHelper.PostAsync(model, "api/Account/Manage/SetUserDetails");
        }

        public async Task<TwoFactorAuthenticationModel> TwoFactorAuthentication(string userId)
        {
            return await _apiHelper.GetFromUri<TwoFactorAuthenticationModel>($"api/Account/Manage/TwoFactorAuthentication/{userId}");
        }

        public async Task<VerifyTwoFactorTokenModel> VerifyTwoFactorTokenAsync(VerifyTwoFactorTokenModel model)
        {
            return await _apiHelper.PostAsync(model, "api/Account/Manage/VerifyTwoFactorToken");
        }
    }
}