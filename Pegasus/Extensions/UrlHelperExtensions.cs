using Microsoft.AspNetCore.Mvc;
using Pegasus.Controllers;

namespace Pegasus.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string ResetPasswordBaseUrl(this IUrlHelper urlHelper, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ResetPassword),
                controller: "Account",
                protocol: scheme,
                values: null);
        }
    }
}
