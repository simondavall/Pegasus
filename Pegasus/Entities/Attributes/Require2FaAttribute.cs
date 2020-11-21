using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Pegasus.Entities.Attributes
{
    public class Require2FaAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var claimTwoFactorEnabled = context.HttpContext.User.Claims.FirstOrDefault(t => t.Type == "amr");

            if (claimTwoFactorEnabled == null || "mfa".Equals(claimTwoFactorEnabled.Value))
            {
                context.Result = new RedirectToActionResult("TwoFactorAuthentication", "Manage", null);
            }
        }
    }
}