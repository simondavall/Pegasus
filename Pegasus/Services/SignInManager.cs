﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Pegasus.Library.Api;
using Pegasus.Library.JwtAuthentication;
using Pegasus.Library.JwtAuthentication.Constants;
using Pegasus.Library.JwtAuthentication.Models;
using Pegasus.Models.Account;
using Pegasus.Services.Models;
using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace Pegasus.Services
{
    public class SignInManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountsEndpoint _accountsEndpoint;
        private readonly IApiHelper _apiHelper;
        private readonly IJwtTokenAccessor _tokenAccessor;

        public SignInManager(IHttpContextAccessor httpContextAccessor, IAccountsEndpoint accountsEndpoint, IApiHelper apiHelper, IJwtTokenAccessor tokenAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _accountsEndpoint = accountsEndpoint;
            _apiHelper = apiHelper;
            _tokenAccessor = tokenAccessor;
        }

        private HttpContext HttpContext
        {
            get { return _httpContextAccessor.HttpContext; }
        }

        public async Task RememberTwoFactorClientAsync(string userId)
        {
            var principal = await StoreRememberClient(userId);
            await HttpContext.SignInAsync(CookieConstants.TwoFactorRememberMeScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });
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
                    additionalClaims.Add(new Claim("amr", "pwd"));
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

        public async Task<string> GetTwoFactorAuthenticationUserAsync()
        {
            var info = await RetrieveTwoFactorInfoAsync();
            return info == null ? null : info.UserId;
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

        private async Task<bool> IsTwoFactorClientRememberedAsync(string userId)
        {
            var result = await HttpContext.AuthenticateAsync(CookieConstants.TwoFactorRememberMeScheme);
            return (result?.Principal != null && result.Principal.FindFirstValue(ClaimTypes.Name) == userId);
        }
    }
}