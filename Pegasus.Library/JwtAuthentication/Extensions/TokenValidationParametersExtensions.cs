using System;
using Microsoft.IdentityModel.Tokens;
using Pegasus.Library.JwtAuthentication.Models;

namespace Pegasus.Library.JwtAuthentication.Extensions
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