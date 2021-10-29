using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.Exceptions;
using PegasusApi.Library.Services.Resources;
using PegasusApi.Models.Account;

namespace PegasusApi.Tests.Controllers.AccountController
{
    public class ForgotPasswordTests : AccountControllerTestsBase
    {
        [Test]
        public async Task ForgotPassword_UserIsNull_ReturnsModel()
        {
            MockUserManager.Setup(x => x.IsEmailConfirmedAsync(It.IsAny<IdentityUser>()));
            MockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()));
            
            var model = new ForgotPasswordModel { Email = EmailAddressNotFound };
            var sut = CreateAccountController();
            var result = await sut.ForgotPassword(model);

            MockUserManager.Verify(x => x.IsEmailConfirmedAsync(It.IsAny<IdentityUser>()), Times.Never);
            MockUserManager.Verify(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()), Times.Never);
            result.Should().BeOfType<ForgotPasswordModel>();
            result.Email.Should().Be(model.Email);
        }

        [Test]
        public async Task ForgotPassword_EmailNotConfirmed_ReturnsModel()
        {
            MockUserManager.Setup(x => x.IsEmailConfirmedAsync(It.IsAny<IdentityUser>())).ReturnsAsync(false);
            MockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()));

            var model = new ForgotPasswordModel { Email = EmailAddressGood };
            var sut = CreateAccountController();
            var result = await sut.ForgotPassword(model);

            MockUserManager.Verify(x => x.IsEmailConfirmedAsync(It.IsAny<IdentityUser>()), Times.Once);
            MockUserManager.Verify(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()), Times.Never);
            result.Should().BeOfType<ForgotPasswordModel>();
            result.Email.Should().Be(model.Email);
        }

        [Test]
        public async Task ForgotPassword_TrySendEmail_CallsSendEmailAndReturnsModel()
        {
            MockUserManager.Setup(x => x.IsEmailConfirmedAsync(It.IsAny<IdentityUser>())).ReturnsAsync(true);
            MockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(string.Empty);
            MockEmailSender.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var model = new ForgotPasswordModel { Email = EmailAddressGood, BaseUrl = "https://example.com" };
            var sut = CreateAccountController();
            var result = await sut.ForgotPassword(model);

            MockUserManager.Verify(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()), Times.Once);
            MockEmailSender.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            result.Should().BeOfType<ForgotPasswordModel>();
            result.Email.Should().Be(model.Email);
            result.IsSuccess.Should().BeTrue();
            result.Errors.Should().BeNull();
        }
        
        [Test]
        public async Task ForgotPassword_EmailException_ReturnsModelWithErrors()
        {
            var exception = new EmailApiKeyNotFoundException(Resources.ErrorStrings.NoEmailApiKeyFound);
            MockUserManager.Setup(x => x.IsEmailConfirmedAsync(It.IsAny<IdentityUser>())).ReturnsAsync(true);
            MockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(string.Empty);
            MockEmailSender.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(exception);

            var model = new ForgotPasswordModel { Email = EmailAddressGood, BaseUrl = "https://example.com" };
            var sut = CreateAccountController();
            var result = await sut.ForgotPassword(model);

            MockUserManager.Verify(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()), Times.Once);
            MockEmailSender.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            result.Should().BeOfType<ForgotPasswordModel>();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Count().Should().Be(1);
            result.Errors.FirstOrDefault().Should().Be(Resources.ErrorStrings.NoEmailApiKeyFound);
        }
    }
}