using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Pegasus.Library.Api;
using Pegasus.Library.JwtAuthentication;
using Pegasus.Library.JwtAuthentication.Constants;
using Pegasus.Library.JwtAuthentication.Models;
using Pegasus.Library.Models.Account;
using Pegasus.Models.Account;
using Pegasus.Services.Models;
using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace Pegasus.Services
{
    public interface ISignInManager
    {
        Task DoSignInAsync(string userId, bool rememberClient);
        Task DoTwoFactorSignInAsync(string userId, bool rememberClient);
        Task ForgetTwoFactorClientAsync();
        Task<string> GetTwoFactorAuthenticationUserAsync();
        Task<bool> IsTwoFactorClientRememberedAsync(string userId);
        Task RefreshSignInAsync(string userId);
        Task<SignInResultModel> SignInOrTwoFactor(AuthenticatedUser authenticatedUser);
        Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode);
    }

    public class SignInManager : ISignInManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountsEndpoint _accountsEndpoint;
        private readonly IApiHelper _apiHelper;
        private readonly IJwtTokenAccessor _tokenAccessor;
        private readonly IAuthenticationEndpoint _authenticationEndpoint;

        public SignInManager(IHttpContextAccessor httpContextAccessor, IAccountsEndpoint accountsEndpoint, IApiHelper apiHelper,
            IJwtTokenAccessor tokenAccessor, IAuthenticationEndpoint authenticationEndpoint)
        {
            _httpContextAccessor = httpContextAccessor;
            _accountsEndpoint = accountsEndpoint;
            _apiHelper = apiHelper;
            _tokenAccessor = tokenAccessor;
            _authenticationEndpoint = authenticationEndpoint;
        }

        private HttpContext HttpContext
        {
            get { return _httpContextAccessor.HttpContext; }
        }

        public async Task DoSignInAsync(string userId, bool rememberClient)
        {
            var authenticatedUser = await _authenticationEndpoint.Authenticate(userId);
            await DoSignInAsync(authenticatedUser, rememberClient);
        }

        public async Task DoTwoFactorSignInAsync(string userId, bool rememberClient)
        {
            var authenticatedUser = await _authenticationEndpoint.Authenticate2Fa(userId);
            await DoSignInAsync(authenticatedUser, rememberClient);
        }

        /// <summary>
        /// Clears the "Remember this browser flag" from the current browser, as an asynchronous operation.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task ForgetTwoFactorClientAsync()
        {
            return HttpContext.SignOutAsync(CookieConstants.TwoFactorRememberMeScheme);
        }

        public async Task<string> GetTwoFactorAuthenticationUserAsync()
        {
            var info = await RetrieveTwoFactorInfoAsync();
            return info == null ? null : info.UserId;
        }

        /// <summary>
        /// Returns a flag indicating if the current client browser has been remembered by two factor authentication
        /// for the user attempting to login, as an asynchronous operation.
        /// </summary>
        /// <param name="userId">The user attempting to login.</param>
        /// <returns>
        /// The task object representing the asynchronous operation containing true if the browser has been remembered
        /// for the current user.
        /// </returns>
        public async Task<bool> IsTwoFactorClientRememberedAsync(string userId)
        {
            var result = await HttpContext.AuthenticateAsync(CookieConstants.TwoFactorRememberMeScheme);
            return (result?.Principal != null && result.Principal.FindFirstValue(ClaimTypes.Name) == userId);
        }

        /// <summary>
        /// Regenerates the user's application cookie, whilst preserving the existing
        /// AuthenticationProperties like rememberMe, as an asynchronous operation.
        /// </summary>
        /// <param name="userId">The user whose sign-in cookie should be refreshed.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task RefreshSignInAsync(string userId)
        {
            var auth = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var uses2Fa = auth?.Principal?.FindFirst("amr");
            if (uses2Fa != null)
            {
                await DoTwoFactorSignInAsync(userId, false);
            }

            await DoSignInAsync(userId, false);
        }

        public async Task<SignInResultModel> SignInOrTwoFactor(AuthenticatedUser authenticatedUser)
        {
            var additionalClaims = new List<Claim>();
            var signInResultModel = new SignInResultModel();
            var accessTokenResult = _tokenAccessor.GetAccessTokenWithClaimsPrincipal(authenticatedUser);
            if (authenticatedUser.RequiresTwoFactor)
            {
                if (await IsTwoFactorClientRememberedAsync(authenticatedUser.UserId))
                {
                    additionalClaims.Add(new Claim("amr", "mfa"));
                }
                else
                {
                    await HttpContext.SignInAsync(CookieConstants.TwoFactorUserIdScheme, StoreTwoFactorInfo(authenticatedUser.UserId, null));
                    signInResultModel.RequiresTwoFactor = true;
                    return signInResultModel;
                }
            }

            await SignInWithClaimsAsync(accessTokenResult.ClaimsPrincipal, accessTokenResult.AuthenticationProperties, additionalClaims);
            _apiHelper.AddTokenToHeaders(accessTokenResult.AccessToken);
            signInResultModel.Success = true;

            return signInResultModel;
        }

        public async Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode)
        {
            var twoFactorInfo = await RetrieveTwoFactorInfoAsync();
            if (twoFactorInfo == null || twoFactorInfo.UserId == null)
            {
                return SignInResult.Failed;
            }

            var model = new RedeemTwoFactorRecoveryCodeModel
            {
                UserId = twoFactorInfo.UserId,
                RecoveryCode = recoveryCode
            };

            var result = await _accountsEndpoint.RedeemTwoFactorRecoveryCodeAsync(model);
            if (result.Succeeded)
            {
                await DoTwoFactorSignInAsync(twoFactorInfo.UserId, false);
                return SignInResult.Success;
            }

            // We don't protect against brute force attacks since codes are expected to be random.
            return SignInResult.Failed;
        }

        private async Task DoSignInAsync(AuthenticatedUser authenticatedUser, bool rememberClient)
        {
            var accessTokenResult = _tokenAccessor.GetAccessTokenWithClaimsPrincipal(authenticatedUser);
            if (rememberClient)
            {
                await RememberTwoFactorClientAsync(authenticatedUser.UserId);
            }

            await HttpContext.SignOutAsync();
            await HttpContext.SignInAsync(accessTokenResult.ClaimsPrincipal, accessTokenResult.AuthenticationProperties);

            _apiHelper.AddTokenToHeaders(accessTokenResult.AccessToken);
        }

        private async Task RememberTwoFactorClientAsync(string userId)
        {
            var principal = await StoreRememberClient(userId);
            await HttpContext.SignInAsync(CookieConstants.TwoFactorRememberMeScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });
        }

        private async Task<TwoFactorAuthenticationInfo> RetrieveTwoFactorInfoAsync()
        {
            var result = await HttpContext.AuthenticateAsync(CookieConstants.TwoFactorUserIdScheme);
            if (result?.Principal != null)
            {
                return new TwoFactorAuthenticationInfo
                {
                    UserId = result.Principal.FindFirstValue(ClaimTypes.Name),
                    LoginProvider = result.Principal.FindFirstValue(ClaimTypes.AuthenticationMethod)
                };
            }
            return null;
        }

        private async Task SignInWithClaimsAsync(ClaimsPrincipal principal, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims = null)
        {
            if (additionalClaims != null)
            {
                foreach (var claim in additionalClaims)
                {
                    principal.Identities.First().AddClaim(claim);
                }
            }

            await HttpContext.SignInAsync(principal, authenticationProperties ?? new AuthenticationProperties());
        }

        private async Task<ClaimsPrincipal> StoreRememberClient(string userId)
        {
            var rememberClient = await _accountsEndpoint.RememberClientAsync(userId);
            var rememberBrowserIdentity = new ClaimsIdentity(CookieConstants.TwoFactorRememberMeScheme);
            rememberBrowserIdentity.AddClaim(new Claim(ClaimTypes.Name, userId));
            if (rememberClient.SupportsUserSecurityStamp)
            {
                var stamp = rememberClient.SecurityStamp;
                var claimsIdentityOptions = new ClaimsIdentityOptions();
                rememberBrowserIdentity.AddClaim(new Claim(claimsIdentityOptions.SecurityStampClaimType, stamp));
            }

            return new ClaimsPrincipal(rememberBrowserIdentity);
        }

        private static ClaimsPrincipal StoreTwoFactorInfo(string userId, string loginProvider)
        {
            var identity = new ClaimsIdentity(CookieConstants.TwoFactorUserIdScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, userId));
            if (loginProvider != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, loginProvider));
            }
            return new ClaimsPrincipal(identity);
        }
    }
}
