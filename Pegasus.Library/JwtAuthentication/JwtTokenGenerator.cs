using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Pegasus.Library.JwtAuthentication.Models;


namespace Pegasus.Library.JwtAuthentication
{
    /// <summary>
    /// A generic Json Web Token generator for use with token based authentication in web applications
    /// </summary>
    public sealed class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly TokenOptions _tokenOptions;

        /// <summary>
        /// Create a token generator instance
        /// </summary>
        /// <param name="tokenOptions"></param>

        public JwtTokenGenerator(TokenOptions tokenOptions)
        {
            this._tokenOptions = tokenOptions ??
                throw new ArgumentNullException(
                    $"An instance of valid {nameof(TokenOptions)} must be passed in order to generate a JWT!");
        }

        public string GenerateAccessToken(string userName, IEnumerable<Claim> userClaims)
        {
            var expiration = TimeSpan.FromMinutes(_tokenOptions.TokenExpiryInMinutes);
            var jwt = new JwtSecurityToken(issuer: _tokenOptions.Issuer,
                                           audience: _tokenOptions.Audience,
                                           claims: MergeUserClaimsWithDefaultClaims(userName, userClaims),
                                           notBefore: DateTime.UtcNow,
                                           expires: DateTime.UtcNow.Add(expiration),
                                           signingCredentials: new SigningCredentials(
                                               _tokenOptions.SigningKey,
                                               SecurityAlgorithms.HmacSha256));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            return accessToken;
        }

        public TokenWithClaimsPrincipal GenerateAccessTokenWithClaimsPrincipal(string userName, IEnumerable<Claim> userClaims)
        {
            var userClaimList = userClaims.ToList();
            var accessToken = GenerateAccessToken(userName, userClaimList);

            return new TokenWithClaimsPrincipal()
            {
                AccessToken = accessToken,
                ClaimsPrincipal = ClaimsPrincipalFactory.CreatePrincipal(
                    MergeUserClaimsWithDefaultClaims(userName, userClaimList)),
                AuthenticationProperties = CreateAuthenticationProperties(accessToken)
            };
        }

        //TODO Change the create token above to a GetToken where the create is actually retrieved from the api.
        //public TokenWithClaimsPrincipal GetAccessTokenWithClaimsPrincipal(string accessToken, string username, IEnumerable<Claim> userClaims)
        //{
        //    var userClaimList = userClaims.ToList();
        //    //var accessToken = GenerateAccessToken(userName, userClaimList);

        //    return new TokenWithClaimsPrincipal()
        //    {
        //        AccessToken = accessToken,
        //        ClaimsPrincipal = ClaimsPrincipalFactory.CreatePrincipal(
        //            MergeUserClaimsWithDefaultClaims(username, userClaimList)),
        //        AuthenticationProperties = CreateAuthenticationProperties(accessToken)
        //    };
        //}

        public static ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            var validationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySecretKeyIsSecretSoDoNotTell"))
            };

            //validationParameters.ValidAudience = _audience.ToLower();
            //validationParameters.ValidIssuer = _issuer.ToLower();

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

        private static IEnumerable<Claim> MergeUserClaimsWithDefaultClaims(string userName, IEnumerable<Claim> userClaims)
        {
            var claims = new List<Claim>(userClaims)
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            return claims;
        }
    }
}