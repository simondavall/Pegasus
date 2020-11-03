using System;
using Microsoft.Extensions.DependencyInjection;
using PegasusApi.Library.JwtAuthentication.Models;

namespace PegasusApi.Library.JwtAuthentication.Extensions
{
    /// <summary>
    /// Simple extension class to encapsulate data protection and cookie auth boilerplate.
    /// </summary>
    public static class ServiceCollectionExtensions
    {       
        public static IServiceCollection AddJwtAuthenticationForApi(
            this IServiceCollection services,
            TokenOptions tokenOptions)
        {
            if (tokenOptions == null)
            {
                throw new ArgumentNullException(
                    $"{nameof(tokenOptions)} is a required parameter. " +
                    $"Please make sure you've provided a valid instance with the appropriate values configured.");
            }

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>(serviceProvider =>
                new JwtTokenGenerator(tokenOptions));

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "JwtBearer";
                    options.DefaultChallengeScheme = "JwtBearer";
                })
                .AddJwtBearer("JwtBearer", options =>
                {
                    options.TokenValidationParameters = tokenOptions.ToTokenValidationParams();
                });

            return services;
        }
    }
}
