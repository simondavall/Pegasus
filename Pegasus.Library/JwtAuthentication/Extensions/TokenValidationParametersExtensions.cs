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
                ClockSkew = TimeSpan.Zero,

                ValidateAudience = true,
                ValidAudience = tokenOptions.Audience,

                ValidateIssuer = true,
                ValidIssuer = tokenOptions.Issuer,

                IssuerSigningKey = tokenOptions.SigningKey,
                ValidateIssuerSigningKey = true,

                RequireExpirationTime = true,
                ValidateLifetime = true
            };
    }
}