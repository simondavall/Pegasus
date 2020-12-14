using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace PegasusApi.Library.JwtAuthentication
{
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Generate a string JSON Web Token containing the claims passed in. Use this method for
        /// ASP.NET Core Web API applications with cookieless authentication.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userClaims"></param>
        /// <returns><see cref="string"/>a plain JWT</returns>
        string GenerateAccessToken(IdentityUser user, IEnumerable<Claim> userClaims);
    }
}