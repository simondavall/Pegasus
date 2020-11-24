using System.Threading.Tasks;
using Pegasus.Library.Models.Manage;

namespace Pegasus.Library.Api
{
    public interface IManageEndpoint
    {
        Task<TwoFactorAuthenticationModel> TwoFactorAuthentication(string email);
        Task ForgetTwoFactorClientAsync(string email);
        Task<AuthenticatorKeyModel> LoadSharedKeyAndQrCodeUriAsync(string email);
        Task<VerifyTwoFactorTokenModel> VerifyTwoFactorTokenAsync(VerifyTwoFactorTokenModel model);
        Task<SetTwoFactorEnabledModel> SetTwoFactorEnabledAsync(SetTwoFactorEnabledModel model);
        Task<GetTwoFactorEnabledModel> GetTwoFactorEnabledAsync(string email);
        Task<GenerateRecoveryCodesModel> GenerateNewRecoveryCodesAsync(string email);
        Task<ResetAuthenticatorModel> ResetAuthenticatorAsync(ResetAuthenticatorModel model);
        Task<RecoveryCodeStatusModel> CheckRecoveryCodesStatus(RecoveryCodeStatusModel model);
    }
}