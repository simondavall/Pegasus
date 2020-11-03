﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Pegasus.Library.JwtAuthentication.Extensions;
using Pegasus.Library.JwtAuthentication.Models;
using Pegasus.Library.Models;


namespace Pegasus.Library.JwtAuthentication
{
    public sealed class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly TokenOptions _tokenOptions;

        public JwtTokenGenerator(TokenOptions tokenOptions)
        {
            this._tokenOptions = tokenOptions ??
                throw new ArgumentNullException(
                    $"An instance of valid {nameof(TokenOptions)} must be passed in order to generate a JWT!");
        }

        public TokenWithClaimsPrincipal GetAccessTokenWithClaimsPrincipal(AuthenticatedUser user)
        {
            var accessToken = user.AccessToken;
            var validationParams = _tokenOptions.ToTokenValidationParams();
            var claimsPrincipal = ValidateToken(accessToken, validationParams);
            return new TokenWithClaimsPrincipal()
            {
                AccessToken = accessToken,
                ClaimsPrincipal = claimsPrincipal,
                AuthenticationProperties = CreateAuthenticationProperties(accessToken)
            };
        }

        private ClaimsPrincipal ValidateToken(string jwtToken, TokenValidationParameters validationParameters)
        {
            IdentityModelEventSource.ShowPII = true;

            // ReSharper disable once NotAccessedVariable
            SecurityToken validatedToken;
            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }

        private static AuthenticationProperties CreateAuthenticationProperties(string accessToken)
        {
            var authProps = new AuthenticationProperties();
            authProps.StoreTokens(
                new[]
                {
                    new AuthenticationToken
                    {
                        Name = TokenConstants.TokenName,
                        Value = accessToken
                    }
                });

            return authProps;
        }
    }
}