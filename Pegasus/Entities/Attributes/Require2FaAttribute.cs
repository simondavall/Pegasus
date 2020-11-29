using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pegasus.Controllers;
using Pegasus.Library.JwtAuthentication.Constants;

namespace Pegasus.Entities.Attributes
{
    public class Require2FaAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private const string ActionName = nameof(ManageController.TwoFactorAuthentication);
        private readonly string _controllerName = nameof(ManageController).Replace(nameof(Controller), string.Empty);
        private const object RouteValues = null;
        private const string MultiFactorAuthentication = "mfa";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var amrClaims = context.HttpContext.User.Claims.Where(t => t.Type == ClaimTypes.Amr);

            if (amrClaims.All(t => t.Value != MultiFactorAuthentication))
            {
                context.Result = new RedirectToActionResult(ActionName, _controllerName, RouteValues);
            }
        }
    }
}