using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using PegasusApi.Models.Account;

namespace PegasusApi.Controllers
{
    [Authorize(Roles = "PegasusUser")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        [Route("ForgotPassword")]
        [HttpPost]
        public async Task<ForgotPasswordModel> ForgotPassword(ForgotPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Input.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return model;
            }

            // For more information on how to enable account confirmation and password reset please 
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var queryStringParams = new Dictionary<string, string>()
            {
                {"userId", user.Id},
                {"code", code}
            };
            var callbackUrl = new Uri(QueryHelpers.AddQueryString(model.BaseUrl, queryStringParams));

            await _emailSender.SendEmailAsync(
                model.Input.Email,
                "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl.ToString())}'>clicking here</a>.");

            //TODO Probably don't need to send anything back
            return model;
        }

        [AllowAnonymous]
        [Route("ResetPassword")]
        [HttpPost]
        public async Task<ResetPasswordModel> ResetPassword(ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Input.Email);
            if (user == null || user.Id != model.Input.UserId)
            {
                // Don't reveal that the user does not exist
                return new ResetPasswordModel {Succeeded = true};
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Input.Code, model.Input.Password);
            if (result.Succeeded)
            {
                return new ResetPasswordModel {Succeeded = true};
            }

            return new ResetPasswordModel {Errors = result.Errors};
        }

        [Route("VerifyTwoFactorToken")]
        [HttpPost]
        public async Task<VerifyTwoFactorModel> VerifyTwoFactorToken(VerifyTwoFactorModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var tokenOptions = new TokenOptions();
            model.Verified = await _userManager.VerifyTwoFactorTokenAsync(user, tokenOptions.AuthenticatorTokenProvider, model.Code);
            return model;
        }
    }
}
