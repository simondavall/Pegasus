using Microsoft.Extensions.Configuration;
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
            IConfiguration configuration)
        {
            var tokenOptions = new TokenOptions(
                configuration["Token:Audience"],
                configuration["Token:Issuer"],
                configuration["Token:SigningKey"],
                configuration["Token:ExpiryInMinutes"]);

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
