using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Moq;
using NUnit.Framework;
using PegasusApi.Controllers;
using PegasusApi.Models.Account;

namespace PegasusApi.Tests.Controllers
{
    public class AccountControllerTests
    {
        private Mock<IEmailSender> _mockEmailSender;
        private Mock<UserManager<IdentityUser>> _mockUserManager;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            //base.OneTimeSetup();
        }

        [SetUp]
        public void EachTestSetup()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager =
                new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            _mockEmailSender = new Mock<IEmailSender>();
        }

        private AccountController CreateAccountController()
        {
            return new AccountController(_mockUserManager.Object, _mockEmailSender.Object);
        }

        [Test]
        public async Task ForgotPassword_UserIsNull_ReturnsModel()
        {
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);
            _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()));

            var model = new ForgotPasswordModel { Email = "test@email.com" };
            var sut = CreateAccountController();
            var result = await sut.ForgotPassword(model);

            _mockUserManager.Verify(x => x.IsEmailConfirmedAsync(It.IsAny<IdentityUser>()), Times.Never);
            _mockUserManager.Verify(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()), Times.Never);
            result.Should().BeOfType<ForgotPasswordModel>();
            result.Email.Should().Be(model.Email);
        }

        [Test]
        public async Task ForgotPassword_EmailNotConfirmed_ReturnsModel()
        {
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new IdentityUser());
            _mockUserManager.Setup(x => x.IsEmailConfirmedAsync(It.IsAny<IdentityUser>())).ReturnsAsync(false);
            _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()));

            var model = new ForgotPasswordModel { Email = "test@email.com" };
            var sut = CreateAccountController();
            var result = await sut.ForgotPassword(model);

            _mockUserManager.Verify(x => x.IsEmailConfirmedAsync(It.IsAny<IdentityUser>()), Times.Once);
            _mockUserManager.Verify(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()), Times.Never);
            result.Should().BeOfType<ForgotPasswordModel>();
            result.Email.Should().Be(model.Email);
        }

        [Test]
        public async Task ForgotPassword_TrySendEmail_CallsSendEmailAndReturnsModel()
        {
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new IdentityUser());
            _mockUserManager.Setup(x => x.IsEmailConfirmedAsync(It.IsAny<IdentityUser>())).ReturnsAsync(true);
            _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(string.Empty);
            _mockEmailSender.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var model = new ForgotPasswordModel { Email = "test@email.com", BaseUrl = "https://example.com" };
            var sut = CreateAccountController();
            var result = await sut.ForgotPassword(model);

            _mockUserManager.Verify(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()), Times.Once);
            _mockEmailSender.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
            result.Should().BeOfType<ForgotPasswordModel>();
            result.Email.Should().Be(model.Email);
        }

        [Test]
        public async Task RedeemTwoFactorRecoveryCode_UserIsNull_ReturnsSucceededFalse()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);
            _mockUserManager.Setup(
                x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()));

            var model = new RedeemTwoFactorRecoveryCodeModel { UserId = "user-id" };
            var sut = CreateAccountController();
            var result = await sut.RedeemTwoFactorRecoveryCode(model);

            _mockUserManager.Verify(
                x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<RedeemTwoFactorRecoveryCodeModel>();
            result.Succeeded.Should().BeFalse();
        }

        [Test]
        public async Task RedeemTwoFactorRecoveryCode_UserIdIsNull_ReturnsSucceededFalse()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);

            var model = new RedeemTwoFactorRecoveryCodeModel();
            var sut = CreateAccountController();
            var result = await sut.RedeemTwoFactorRecoveryCode(model);

            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<RedeemTwoFactorRecoveryCodeModel>();
            result.Succeeded.Should().BeFalse();
        }

        [Test]
        public async Task RedeemTwoFactorRecoveryCode_UserNotFound_ReturnsSucceededFalse()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);
            _mockUserManager.Setup(
                x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()));

            var model = new RedeemTwoFactorRecoveryCodeModel { UserId = "user-id" };
            var sut = CreateAccountController();
            var result = await sut.RedeemTwoFactorRecoveryCode(model);

            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(
                x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<RedeemTwoFactorRecoveryCodeModel>();
            result.Succeeded.Should().BeFalse();
        }

        [Test]
        public async Task RedeemTwoFactorRecoveryCode_RedeemHasErrors_ReturnsSucceededFalse()
        {
            var errors = new[] { new IdentityError { Code = string.Empty, Description = string.Empty } };

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new IdentityUser());
            _mockUserManager
                .Setup(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            var model = new RedeemTwoFactorRecoveryCodeModel { UserId = "user-id" };
            var sut = CreateAccountController();
            var result = await sut.RedeemTwoFactorRecoveryCode(model);

            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(
                x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            result.Should().BeOfType<RedeemTwoFactorRecoveryCodeModel>();
            result.Succeeded.Should().BeFalse();
        }

        [Test]
        public async Task RedeemTwoFactorRecoveryCode_RedeemSuccess_ReturnsSucceededTrue()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new IdentityUser());
            _mockUserManager
                .Setup(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var model = new RedeemTwoFactorRecoveryCodeModel { UserId = "user-id" };
            var sut = CreateAccountController();
            var result = await sut.RedeemTwoFactorRecoveryCode(model);

            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(
                x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            result.Should().BeOfType<RedeemTwoFactorRecoveryCodeModel>();
            result.Succeeded.Should().BeTrue();
        }

        [Test]
        public async Task RememberClient_UserIsNull_ReturnsModel()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);

            var sut = CreateAccountController();
            var result = await sut.RememberClient(new RememberClientModel());

            result.Should().BeOfType<RememberClientModel>();
            result.SecurityStamp.Should().BeNull();
        }

        [Test]
        public async Task RememberClient_UserWithSecurityStamp_ReturnsModelWithSecurityStamp()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser { SecurityStamp = "security-stamp" });

            var sut = CreateAccountController();
            var result = await sut.RememberClient(new RememberClientModel { UserId = "user-id" });

            result.Should().BeOfType<RememberClientModel>();
            result.SecurityStamp.Should().Be("security-stamp");
        }

        [Test]
        public async Task ResetPassword_NoEmailAddress_ReturnsNoEmailError()
        {
            var sut = CreateAccountController();
            var result = await sut.ResetPassword(new ResetPasswordModel());

            result.Should().BeOfType<ResetPasswordModel>();
            result.Errors.Should().NotBeNull();
            result.Errors.Count().Should().Be(1);
            //TODO Put this string in Resources.
            result.Errors.First().Description.Should().Be("Email not supplied");
        }

        [Test]
        public async Task ResetPassword_UserIsNull_ReturnsSucceededButDoesNotResetPassword()
        {
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);
            _mockUserManager.Setup(x =>
                    x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var model = new ResetPasswordModel { UserId = "user-id", Email = "test@example.com" };
            var sut = CreateAccountController();
            var result = await sut.ResetPassword(model);

            _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(
                x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
            result.Should().BeOfType<ResetPasswordModel>();
            result.Succeeded.Should().BeTrue();
        }

        [Test]
        public async Task ResetPassword_UserNoSameIdAsModel_ReturnsSucceededButDoesNotResetPassword()
        {
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser { Id = "different-id" });
            _mockUserManager.Setup(x =>
                    x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var model = new ResetPasswordModel { UserId = "user-id", Email = "test@example.com" };
            var sut = CreateAccountController();
            var result = await sut.ResetPassword(model);

            _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(
                x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
            result.Should().BeOfType<ResetPasswordModel>();
            result.Succeeded.Should().BeTrue();
        }

        [Test]
        public async Task ResetPassword_ResetFailed_ReturnsModelWithErrors()
        {
            var errors = new[] { new IdentityError { Code = string.Empty, Description = string.Empty } };

            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser { Id = "user-id" });
            _mockUserManager.Setup(x =>
                    x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            var model = new ResetPasswordModel { UserId = "user-id", Email = "test@example.com" };
            var sut = CreateAccountController();
            var result = await sut.ResetPassword(model);

            _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(
                x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
            result.Should().BeOfType<ResetPasswordModel>();
            result.Succeeded.Should().BeFalse();
            result.Errors.Should().NotBeNull();
            result.Errors.Any().Should().BeTrue();
        }

        [Test]
        public async Task ResetPassword_ResetSuccess_ReturnsModelWithNoErrors()
        {
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser { Id = "user-id" });
            _mockUserManager.Setup(x =>
                    x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var model = new ResetPasswordModel { UserId = "user-id", Email = "test@example.com" };
            var sut = CreateAccountController();
            var result = await sut.ResetPassword(model);

            _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(
                x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
            result.Should().BeOfType<ResetPasswordModel>();
            result.Succeeded.Should().BeTrue();
            result.Errors.Should().BeNull();
        }

        [Test]
        public async Task VerifyTwoFactorToken_ModelIsMull_ReturnsEmptyModel()
        {
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser { Id = "user-id" });

            var sut = CreateAccountController();
            var result = await sut.VerifyTwoFactorToken(null);

            _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<VerifyTwoFactorModel>();
            result.UserId.Should().BeNull();
        }

        [Test]
        public async Task VerifyTwoFactorToken_UserIsMull_ReturnsNotVerified()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);
            _mockUserManager.Setup(x =>
                    x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var sut = CreateAccountController();
            var result = await sut.VerifyTwoFactorToken(new VerifyTwoFactorModel { UserId = "user-id" });

            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(
                x => x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
            result.Should().BeOfType<VerifyTwoFactorModel>();
            result.Verified.Should().BeFalse();
        }

        [Test]
        public async Task VerifyTwoFactorToken_NotVerified_ReturnsNotVerifiedIsFalse()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser { Id = "user-id" });
            _mockUserManager.Setup(x =>
                    x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var sut = CreateAccountController();
            var result = await sut.VerifyTwoFactorToken(new VerifyTwoFactorModel { UserId = "user-id" });

            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(
                x => x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
            result.Should().BeOfType<VerifyTwoFactorModel>();
            result.Verified.Should().BeFalse();
        }

        [Test]
        public async Task VerifyTwoFactorToken_Verified_ReturnsNotVerifiedIsTrue()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser { Id = "user-id" });
            _mockUserManager.Setup(x =>
                    x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var sut = CreateAccountController();
            var result = await sut.VerifyTwoFactorToken(new VerifyTwoFactorModel { UserId = "user-id" });

            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(
                x => x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
            result.Should().BeOfType<VerifyTwoFactorModel>();
            result.Verified.Should().BeTrue();
        }
    }
}