using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pegasus.Library.Api;
using Pegasus.Library.Models;
using Pegasus.Models.Account;

namespace Pegasus.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly IApiHelper _apiHelper;
        private readonly ILoggedInUserModel _loggedInUser;

        public AccountController(ILogger<AccountController> logger, IApiHelper apiHelper, ILoggedInUserModel loggedInUser)
        {
            _logger = logger;
            _apiHelper = apiHelper;
            _loggedInUser = loggedInUser;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                //// This doesn't count login failures towards account lockout
                //// To enable password failures to trigger account lockout, set lockoutOnFailure: true
                //var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                //TODO Nothing assigned to the Remember Me checkbox at the moment
                //TODO Consider enabling lockoutOnFailure
                var result = await _apiHelper.Authenticate(model.Email, model.Password);

                if (result.Succeeded)
                {
                    _loggedInUser.Username = result.Username;
                    _loggedInUser.Token = result.AccessToken;
                    _loggedInUser.IsLoggedIn = true;

                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                //TODO Add this next line if we need 2fa
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToAction(nameof(LoginWith2Fa), new { returnUrl, model.RememberMe });
                //}
                // TODO Implement Lockout recovery
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
        [AllowAnonymous]
        public IActionResult Logout(string returnUrl = null)
        {
            _loggedInUser.Username = string.Empty;
            _loggedInUser.Token = string.Empty;
            _loggedInUser.IsLoggedIn = false;

            _logger.LogInformation("User logged in.");
            ViewData["ReturnUrl"] = returnUrl;
            return RedirectToLocal("/Account/Login");
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
