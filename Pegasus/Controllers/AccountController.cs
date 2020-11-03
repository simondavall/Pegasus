using System.Collections.Generic;
using System.Security.Claims;
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
    public class AccountController : Controller
    {
        private readonly ILogger _logger;
        private readonly IApiHelper _apiHelper;
        private readonly ILoggedInUserModel _loggedInUser;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public AccountController(ILogger<AccountController> logger, IApiHelper apiHelper, 
            ILoggedInUserModel loggedInUser, IJwtTokenGenerator tokenGenerator)
        {
            _logger = logger;
            _apiHelper = apiHelper;
            _loggedInUser = loggedInUser;
            _tokenGenerator = tokenGenerator;
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
                //TODO PGS-71 Nothing assigned to the Remember Me checkbox at the moment
                //TODO PGS-72 Consider enabling lockoutOnFailure
                var result = await _apiHelper.Authenticate(model.Email, model.Password);

                if (result.Succeeded)
                {
                    _loggedInUser.Username = result.Username;
                    _loggedInUser.Token = result.AccessToken;
                    _loggedInUser.IsLoggedIn = true;

                    _logger.LogInformation("User logged in.");

                    var accessTokenResult = _tokenGenerator.GetAccessTokenWithClaimsPrincipal(result);
                    await HttpContext.SignInAsync(accessTokenResult.ClaimsPrincipal, accessTokenResult.AuthenticationProperties);

                    return RedirectToLocal(returnUrl);
                }
                //TODO PGS-73 Add this next line if we need 2fa
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToAction(nameof(LoginWith2Fa), new { returnUrl, model.RememberMe });
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


        //TODO Should this be POST only?
        [Authorize]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            _loggedInUser.Username = string.Empty;
            _loggedInUser.Token = string.Empty;
            _loggedInUser.IsLoggedIn = false;

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

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

        private static IEnumerable<Claim> AddMyClaims(UserInfo authenticatedUser)
        {
            var myClaims = new List<Claim>
            {
                new Claim(ClaimTypes.GivenName, authenticatedUser.FirstName),
                new Claim(ClaimTypes.Surname, authenticatedUser.LastName),
                new Claim("HasAdminRights", authenticatedUser.HasAdminRights ? "Y" : "N")
            };

            return myClaims;
        }
    }

    public class UserInfo
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool HasAdminRights { get; set; }
    }

    public class UserCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


}
