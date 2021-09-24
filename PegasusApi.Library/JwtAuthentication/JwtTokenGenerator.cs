﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ardalis.GuardClauses;
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

        public JwtTokenGenerator(TokenOptions tokenOptions)
        {
            Guard.Against.Null(tokenOptions, nameof(tokenOptions));
            Guard.Against.Null(tokenOptions.SigningKey, nameof(tokenOptions.SigningKey));

            _tokenOptions = tokenOptions;
        }

        public string GenerateAccessToken(IdentityUser user, IEnumerable<Claim> userClaims)
        {
            Guard.Against.Null(user, nameof(user));

            var claims = MergeUserClaimsWithDefaultClaims(user, userClaims);

            var token = new JwtSecurityToken(
                new JwtHeader(new SigningCredentials(_tokenOptions.SigningKey, SecurityAlgorithms.HmacSha256)),
                new JwtPayload(claims));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return accessToken;
        }

        private IEnumerable<Claim> MergeUserClaimsWithDefaultClaims(IdentityUser user, IEnumerable<Claim> userClaims)
        {
            var claims = new List<Claim>(userClaims ?? new List<Claim>())
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