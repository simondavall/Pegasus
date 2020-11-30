using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pegasus.Library.JwtAuthentication.Models;
using Pegasus.Library.Models.Account;

namespace Pegasus.Library.Api
{
    public interface IAuthenticationEndpoint
    {
        Task<AuthenticatedUser> Authenticate(UserCredentials credentials);
        Task<AuthenticatedUser> Authenticate(string userId);
        Task<AuthenticatedUser> Authenticate2Fa(string username);
    }

    public class AuthenticationEndpoint : IAuthenticationEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public AuthenticationEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<AuthenticatedUser> Authenticate(UserCredentials credentials)
        {
            var data = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", credentials?.Username),
                    new KeyValuePair<string, string>("password", credentials?.Password)
                });
            return await PostAsync("/token", data);
        }

        [Authorize(Roles = "PegasusUser")]
        public async Task<AuthenticatedUser> Authenticate(string userId)
        {
            var data = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("userId", userId)
                });

            return await PostAsync("/refresh_token", data);
        }

        [Authorize(Roles = "PegasusUser")]
        public async Task<AuthenticatedUser> Authenticate2Fa(string userId)
        {
            var data = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("userId", userId)
                });

            return await PostAsync("/2fa_token", data);
        }

        private async Task<AuthenticatedUser> PostAsync(string requestUri, HttpContent data)
        {
            using (var response = await _apiHelper.ApiClient.PostAsync(requestUri, data))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<AuthenticatedUser>();
                    result.Authenticated = true;
                    return result;
                }
                else
                {
                    var result = new AuthenticatedUser
                    {
                        FailedReason = response.ReasonPhrase
                    };
                    return result;
                }
            }
        }
    }
}
