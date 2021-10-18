using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.Services.Resources;
using PegasusApi.Models.Account;

namespace PegasusApi.Tests.Controllers.AccountController
{
    public class ResetPasswordTests : AccountControllerTestsBase
    {
        
        [Test]
        public async Task ResetPassword_NoEmailAddress_ReturnsNoEmailError()
        {
            var sut = CreateAccountController();
            var result = await sut.ResetPassword(new ResetPasswordModel());

            result.Should().BeOfType<ResetPasswordModel>();
            result.Errors.Should().NotBeNull();
            result.Errors.Count().Should().Be(1);
            result.Errors.First().Description.Should().Be(Resources.ControllerStrings.AccountController.EmailNotSupplied);
        }

        [Test]
        public async Task ResetPassword_UserIsNull_ReturnsSucceededButDoesNotResetPassword()
        {
            MockUserManager.Setup(x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var model = new ResetPasswordModel { UserId = UserId, Email = EmailAddressNotFound };
            var sut = CreateAccountController();
            var result = await sut.ResetPassword(model);

            MockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            MockUserManager.Verify(x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<ResetPasswordModel>();
            result.Succeeded.Should().BeTrue();
        }

        [Test]
        public async Task ResetPassword_UserNoSameIdAsModel_ReturnsSucceededButDoesNotResetPassword()
        {
            MockUserManager.Setup(x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var model = new ResetPasswordModel { UserId = UnknownUserId, Email = EmailAddressGood };
            var sut = CreateAccountController();
            var result = await sut.ResetPassword(model);

            MockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            MockUserManager.Verify(x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<ResetPasswordModel>();
            result.Succeeded.Should().BeTrue();
        }

        [Test]
        public async Task ResetPassword_ResetFailed_ReturnsModelWithErrors()
        {
            var errors = new[] { new IdentityError { Code = string.Empty, Description = string.Empty } };
            
            MockUserManager.Setup(x =>
                    x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            var model = new ResetPasswordModel { UserId = UserId, Email = EmailAddressGood };
            var sut = CreateAccountController();
            var result = await sut.ResetPassword(model);

            MockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            MockUserManager.Verify(x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            result.Should().BeOfType<ResetPasswordModel>();
            result.Succeeded.Should().BeFalse();
            result.Errors.Should().NotBeNull();
            result.Errors.Any().Should().BeTrue();
        }

        [Test]
        public async Task ResetPassword_ResetSuccess_ReturnsModelWithNoErrors()
        {
            MockUserManager.Setup(x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var model = new ResetPasswordModel { UserId = UserId, Email = EmailAddressGood };
            var sut = CreateAccountController();
            var result = await sut.ResetPassword(model);

            MockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            MockUserManager.Verify(x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            result.Should().BeOfType<ResetPasswordModel>();
            result.Succeeded.Should().BeTrue();
            result.Errors.Should().BeNull();
        }

    }
}