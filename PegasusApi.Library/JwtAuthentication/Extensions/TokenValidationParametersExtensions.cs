using System;
using Microsoft.IdentityModel.Tokens;
using PegasusApi.Library.JwtAuthentication.Models;

namespace PegasusApi.Library.JwtAuthentication.Extensions
{
    public static class TokenValidationParametersExtensions
    {
        public static TokenValidationParameters ToTokenValidationParams(
            this TokenOptions tokenOptions) =>
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = tokenOptions.SigningKey,
                ValidateIssuer = true,
                ValidIssuer = tokenOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = tokenOptions.Audience,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
    }
}