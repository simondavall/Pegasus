using System.Threading.Tasks;
using Pegasus.Library.Models;

namespace Pegasus.Library.Api
{
    public interface IAccountsEndpoint
    {
        Task<ForgotPasswordModel> ForgotPassword(ForgotPasswordModel model);
        Task<ResetPasswordModel> ResetPassword(ResetPasswordModel model);
    }

    public class AccountsEndpoint : IAccountsEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public AccountsEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<ForgotPasswordModel> ForgotPassword(ForgotPasswordModel model)
        {
            return await _apiHelper.PostAsync(model,$"api/Account/ForgotPassword");
        }

        public async Task<ResetPasswordModel> ResetPassword(ResetPasswordModel model)
        {
            return await _apiHelper.PostAsync(model, "api/Account/ResetPassword");
        }
    }
}
