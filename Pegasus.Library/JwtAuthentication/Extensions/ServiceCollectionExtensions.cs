using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pegasus.Library.JwtAuthentication.Models;


namespace Pegasus.Library.JwtAuthentication.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtAuthenticationWithProtectedCookie(this IServiceCollection services, IConfiguration configuration,
            string applicationDiscriminator = null, AuthUrlOptions authUrlOptions = null)
        {
            var tokenOptions = new TokenOptions(
                configuration["Token:Audience"],
                configuration["Token:Issuer"],
                configuration["Token:SigningKey"],
                configuration["Token:ExpiryInMinutes"]);

            var hostingEnvironment = services.BuildServiceProvider().GetService<IHostEnvironment>();

            // The JwtAuthTicketFormat representing the cookie needs an IDataProtector and
            // IDataSerializer to correctly encrypt/decrypt and serialize/deserialize the payload
            // respectively. This requirement is enforced by ISecureDataFormat interface in ASP.NET
            // Core. Read more about ASP.NET Core Data Protection API here:
            // https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/
            // NB: This is only required if you're using JWT with Cookie based authentication, for
            //     cookieless auth (such as with a Web API) the data protection and serialization
            //     dependencies won't be needed. You simply need to set the validation params and add
            //     the token generator dependencies and use the right authentication extension below.

            var applicationName = $"{applicationDiscriminator ?? hostingEnvironment.ApplicationName}";

            services.AddDataProtection(options => options.ApplicationDiscriminator = applicationName)
                .SetApplicationName(applicationName);

            services.AddScoped<IDataSerializer<AuthenticationTicket>, TicketSerializer>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>(serviceProvider => new JwtTokenGenerator(tokenOptions));

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    // cookie expiration should be set the same as the token expiry (the default is 5
                    // mins). The token generator doesn't provide auto-refresh of an expired token so the
                    // user will be logged out the next time they try to access a secured endpoint. They
                    // will simply have to re-login and acquire a new token and by extension a new cookie.
                    // Perhaps in the future I can add some kind of hooks in the token generator that can
                    // let the referencing application know that the token has expired and the developer
                    // can then request a new token without the user having to re-login.
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(tokenOptions.TokenExpiryInMinutes);

                    // Specify the TicketDataFormat to use to validate/create the ASP.NET authentication
                    // ticket. Its important that the same validation parameters are passed to this class
                    // so that the token validation works correctly. The framework will call the
                    // appropriate methods in JwtAuthTicketFormat based on whether the cookie is being
                    // sent out or coming in from a previously authenticated user. Please bear in mind
                    // that if the incoming token is invalid (may be it was tampered or spoofed) the
                    // Unprotect() method in JwtAuthTicketFormat will simply return null and the
                    // authentication will fail.
                    options.Cookie.Name = "PegasusAuth";
                    options.TicketDataFormat = new JwtAuthTicketFormat(
                        tokenOptions.ToTokenValidationParams(),
                        services.BuildServiceProvider().GetService<IDataSerializer<AuthenticationTicket>>(),
                        services.BuildServiceProvider().GetDataProtector(new[] { $"{applicationName}-Auth1" }));

                    options.LoginPath = GetPath(authUrlOptions?.LoginPath, "/Account/Login");
                    options.LogoutPath = GetPath(authUrlOptions?.LogoutPath, "/Account/Logout");
                    options.AccessDeniedPath = GetPath(authUrlOptions?.AccessDeniedPath, "/Account/AccessDenied");
                    options.ReturnUrlParameter = authUrlOptions?.ReturnUrlParameter ?? "returnUrl";
                });

            return services;
        }

        private static string GetPath(string pathFromConfig, string defaultPath)
        {
            return pathFromConfig != null
                ? new PathString(pathFromConfig)
                : new PathString(defaultPath);
        }
    }
}