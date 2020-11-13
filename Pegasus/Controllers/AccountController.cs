using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pegasus.Library.Api;
using Pegasus.Library.JwtAuthentication;
using Pegasus.Library.Models;
using Pegasus.Models.Account;

namespace Pegasus.Controllers
{
    [Authorize(Roles = "PegasusUser")]
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly IApiHelper _apiHelper;
        private readonly IJwtTokenAccessor _tokenAccessor;

        public AccountController(ILogger<AccountController> logger, IApiHelper apiHelper, 
            IJwtTokenAccessor tokenAccessor)
        {
            _logger = logger;
            _apiHelper = apiHelper;
            _tokenAccessor = tokenAccessor;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                //// This doesn't count login failures towards account lockout
                //// To enable password failures to trigger account lockout, set lockoutOnFailure: true
                //var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, lockoutOnFailure: false);
                //TODO PGS-72 Consider enabling lockoutOnFailure
                var credentials = new UserCredentials { Username = model.Email, Password = model.Password };
                var result = await _apiHelper.Authenticate(credentials);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    var accessTokenResult = _tokenAccessor.GetAccessTokenWithClaimsPrincipal(result);
                    await HttpContext.SignInAsync(accessTokenResult.ClaimsPrincipal, accessTokenResult.AuthenticationProperties);

                    _apiHelper.AddTokenToHeaders(accessTokenResult.AccessToken);

                    return RedirectToLocal(returnUrl);
                }
                //TODO PGS-73 Add this next line if we need 2fa
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToAction(nameof(LoginWith2Fa), new { returnUrl });
                //}
                // TODO PGS-72 Implement Lockout recovery
                //if (result.IsLockedOut)
                //{
                //    _logger.LogWarning("User account locked out.");
                //    return RedirectToAction(nameof(Lockout));
                //}
                //else
                //{
                //    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                //    return View(model);
                //}
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _apiHelper.RemoveTokenFromHeaders();
            _logger.LogInformation("User logged out.");
            ViewData["ReturnUrl"] = returnUrl;
            return RedirectToLocal("/Account/Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
