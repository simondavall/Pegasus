using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Pegasus.Extensions;
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
        private readonly IAccountsEndpoint _accountsEndpoint;

        public AccountController(ILogger<AccountController> logger, IApiHelper apiHelper,
            IJwtTokenAccessor tokenAccessor, IAccountsEndpoint accountsEndpoint)
        {
            _logger = logger;
            _apiHelper = apiHelper;
            _tokenAccessor = tokenAccessor;
            _accountsEndpoint = accountsEndpoint;
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
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                model.BaseUrl = Url.ResetPasswordBaseUrl(Request.Scheme);
                await _accountsEndpoint.ForgotPassword(model);
                return View("ForgotPasswordConfirmation");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string userId = null, string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                var resetPasswordViewModel = new ResetPasswordModel
                {
                    Input = new ResetPasswordModel.InputModel
                    {
                        UserId = userId, Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                    }
                };

                return View(resetPasswordViewModel);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _accountsEndpoint.ResetPassword(model);

            if (response.Succeeded)
            {
                return View("ResetPasswordConfirmation");
            }

            //TODO Does this not leave us security vulnerable. i.e. Email/User exists?
            foreach (var error in response.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
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
