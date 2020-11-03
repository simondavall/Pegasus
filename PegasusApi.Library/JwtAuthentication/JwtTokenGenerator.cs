using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using PegasusApi.Library.JwtAuthentication.Models;


namespace PegasusApi.Library.JwtAuthentication
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
            _tokenOptions = tokenOptions ??
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

        private static IEnumerable<Claim> MergeUserClaimsWithDefaultClaims(string userName, IEnumerable<Claim> userClaims)
        {
            var claims = new List<Claim>(userClaims)
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            return claims;
        }
    }
}