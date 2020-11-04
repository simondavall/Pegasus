using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TokenOptions = PegasusApi.Library.JwtAuthentication.Models.TokenOptions;


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

        public string GenerateAccessToken(IdentityUser user, IEnumerable<Claim> userClaims)
        {
            var claims = MergeUserClaimsWithDefaultClaims(user, userClaims);

            var jwt = new JwtSecurityToken(
                new JwtHeader(new SigningCredentials(_tokenOptions.SigningKey, SecurityAlgorithms.HmacSha256)),
                new JwtPayload(claims));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
            return accessToken;
        }

        private IEnumerable<Claim> MergeUserClaimsWithDefaultClaims(IdentityUser user, IEnumerable<Claim> userClaims)
        {
            var claims = new List<Claim>(userClaims)
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, "PegasusUser"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _tokenOptions.Issuer),
                new Claim(JwtRegisteredClaimNames.Aud, _tokenOptions.Audience),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddMinutes(_tokenOptions.TokenExpiryInMinutes)).ToUnixTimeSeconds().ToString())
            };

            return claims;
        }
    }
}