using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Moq;
using NUnit.Framework;
using PegasusApi.Controllers;
using PegasusApi.Models.Account;
using ControllerStrings = PegasusApi.Library.Services.Resources.Resources.ControllerStrings.AccountController;


namespace PegasusApi.Tests.Controllers
{
    public class AccountControllerTests
    {
        private Mock<IEmailSender> _mockEmailSender;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private const string EmailAddressGood = "test-good@email.com";
        private const string EmailAddressNotFound = "test-bad@email.com";
        private const string UserId = "user-id";
        private const string UnknownUserId = "unknown-id";
        private const string GoodToken = "good-token";
        private const string BadToken = "bad-token";
        
        [SetUp]
        public void EachTestSetup()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);
            _mockUserManager.Setup(x => x.FindByEmailAsync( EmailAddressGood)).ReturnsAsync(new IdentityUser { Id = UserId });
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);
            _mockUserManager.Setup(x => x.FindByIdAsync(UserId)).ReturnsAsync(new IdentityUser { Id=UserId, SecurityStamp = "security-stamp" });
            
            _mockEmailSender = new Mock<IEmailSender>();
        }

        private AccountController CreateAccountController()
        {
            return new AccountController(_mockUserManager.Object, _mockEmailSender.Object);
        }

        [Test]
        public async Task ForgotPassword_UserIsNull_ReturnsModel()
        {
            _mockUserManager.Setup(x => x.IsEmailConfirmedAsync(It.IsAny<IdentityUser>()));
            _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()));
            
            var model = new ForgotPasswordModel { Email = EmailAddressNotFound };
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
            _mockUserManager.Setup(x => x.IsEmailConfirmedAsync(It.IsAny<IdentityUser>())).ReturnsAsync(false);
            _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()));

            var model = new ForgotPasswordModel { Email = EmailAddressGood };
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
            _mockUserManager.Setup(x => x.IsEmailConfirmedAsync(It.IsAny<IdentityUser>())).ReturnsAsync(true);
            _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(string.Empty);
            _mockEmailSender.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var model = new ForgotPasswordModel { Email = EmailAddressGood, BaseUrl = "https://example.com" };
            var sut = CreateAccountController();
            var result = await sut.ForgotPassword(model);

            _mockUserManager.Verify(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()), Times.Once);
            _mockEmailSender.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            result.Should().BeOfType<ForgotPasswordModel>();
            result.Email.Should().Be(model.Email);
        }

        [Test]
        public async Task RedeemTwoFactorRecoveryCode_UserIsNull_ReturnsSucceededFalse()
        {
            _mockUserManager.Setup(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()));

            var model = new RedeemTwoFactorRecoveryCodeModel { UserId = UnknownUserId };
            var sut = CreateAccountController();
            var result = await sut.RedeemTwoFactorRecoveryCode(model);

            _mockUserManager.Verify(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<RedeemTwoFactorRecoveryCodeModel>();
            result.Succeeded.Should().BeFalse();
        }

        [Test]
        public async Task RedeemTwoFactorRecoveryCode_UserIdIsNull_ReturnsSucceededFalse()
        {
            var sut = CreateAccountController();
            var result = await sut.RedeemTwoFactorRecoveryCode(new RedeemTwoFactorRecoveryCodeModel());

            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<RedeemTwoFactorRecoveryCodeModel>();
            result.Succeeded.Should().BeFalse();
        }

        [Test]
        public async Task RedeemTwoFactorRecoveryCode_UserNotFound_ReturnsSucceededFalse()
        {
            _mockUserManager.Setup(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()));

            var model = new RedeemTwoFactorRecoveryCodeModel { UserId = UnknownUserId };
            var sut = CreateAccountController();
            var result = await sut.RedeemTwoFactorRecoveryCode(model);

            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<RedeemTwoFactorRecoveryCodeModel>();
            result.Succeeded.Should().BeFalse();
        }

        [Test]
        public async Task RedeemTwoFactorRecoveryCode_RedeemHasErrors_ReturnsSucceededFalse()
        {
            var errors = new[] { new IdentityError { Code = string.Empty, Description = string.Empty } };
            
            _mockUserManager
                .Setup(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            var model = new RedeemTwoFactorRecoveryCodeModel { UserId = UserId };
            var sut = CreateAccountController();
            var result = await sut.RedeemTwoFactorRecoveryCode(model);

            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            result.Should().BeOfType<RedeemTwoFactorRecoveryCodeModel>();
            result.Succeeded.Should().BeFalse();
        }

        [Test]
        public async Task RedeemTwoFactorRecoveryCode_RedeemSuccess_ReturnsSucceededTrue()
        {
            _mockUserManager
                .Setup(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var model = new RedeemTwoFactorRecoveryCodeModel { UserId = UserId };
            var sut = CreateAccountController();
            var result = await sut.RedeemTwoFactorRecoveryCode(model);

            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            result.Should().BeOfType<RedeemTwoFactorRecoveryCodeModel>();
            result.Succeeded.Should().BeTrue();
        }

        [Test]
        public async Task RememberClient_UserIsNull_ReturnsModel()
        {
            var sut = CreateAccountController();
            var result = await sut.RememberClient(new RememberClientModel());

            result.Should().BeOfType<RememberClientModel>();
            result.SecurityStamp.Should().BeNull();
        }

        [Test]
        public async Task RememberClient_UserWithSecurityStamp_ReturnsModelWithSecurityStamp()
        {
            var sut = CreateAccountController();
            var result = await sut.RememberClient(new RememberClientModel { UserId = UserId });

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
            result.Errors.First().Description.Should().Be(ControllerStrings.EmailNotSupplied);
        }

        [Test]
        public async Task ResetPassword_UserIsNull_ReturnsSucceededButDoesNotResetPassword()
        {
            _mockUserManager.Setup(x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var model = new ResetPasswordModel { UserId = UserId, Email = EmailAddressNotFound };
            var sut = CreateAccountController();
            var result = await sut.ResetPassword(model);

            _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<ResetPasswordModel>();
            result.Succeeded.Should().BeTrue();
        }

        [Test]
        public async Task ResetPassword_UserNoSameIdAsModel_ReturnsSucceededButDoesNotResetPassword()
        {
            _mockUserManager.Setup(x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var model = new ResetPasswordModel { UserId = UnknownUserId, Email = EmailAddressGood };
            var sut = CreateAccountController();
            var result = await sut.ResetPassword(model);

            _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<ResetPasswordModel>();
            result.Succeeded.Should().BeTrue();
        }

        [Test]
        public async Task ResetPassword_ResetFailed_ReturnsModelWithErrors()
        {
            var errors = new[] { new IdentityError { Code = string.Empty, Description = string.Empty } };
            
            _mockUserManager.Setup(x =>
                    x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            var model = new ResetPasswordModel { UserId = UserId, Email = EmailAddressGood };
            var sut = CreateAccountController();
            var result = await sut.ResetPassword(model);

            _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            result.Should().BeOfType<ResetPasswordModel>();
            result.Succeeded.Should().BeFalse();
            result.Errors.Should().NotBeNull();
            result.Errors.Any().Should().BeTrue();
        }

        [Test]
        public async Task ResetPassword_ResetSuccess_ReturnsModelWithNoErrors()
        {
            _mockUserManager.Setup(x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var model = new ResetPasswordModel { UserId = UserId, Email = EmailAddressGood };
            var sut = CreateAccountController();
            var result = await sut.ResetPassword(model);

            _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.ResetPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            result.Should().BeOfType<ResetPasswordModel>();
            result.Succeeded.Should().BeTrue();
            result.Errors.Should().BeNull();
        }

        [Test]
        public async Task VerifyTwoFactorToken_ModelIsMull_ReturnsEmptyModel()
        {
            var sut = CreateAccountController();
            var result = await sut.VerifyTwoFactorToken(null);
            
            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<VerifyTwoFactorModel>();
            result.UserId.Should().BeNull();
        }

        [Test]
        public async Task VerifyTwoFactorToken_UserIsMull_ReturnsNotVerified()
        {
            _mockUserManager.Setup(x => x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var sut = CreateAccountController();
            var result = await sut.VerifyTwoFactorToken(new VerifyTwoFactorModel { UserId = UnknownUserId });

            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<VerifyTwoFactorModel>();
            result.Verified.Should().BeFalse();
        }

        [Test]
        public async Task VerifyTwoFactorToken_NotVerified_ReturnsNotVerifiedIsFalse()
        {
            _mockUserManager.Setup(x => x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), BadToken))
                .ReturnsAsync(false);

            var sut = CreateAccountController();
            var result = await sut.VerifyTwoFactorToken(new VerifyTwoFactorModel { UserId = UserId, Code = BadToken});

            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), BadToken), Times.Once);
            result.Should().BeOfType<VerifyTwoFactorModel>();
            result.Verified.Should().BeFalse();
        }

        [Test]
        public async Task VerifyTwoFactorToken_Verified_ReturnsNotVerifiedIsTrue()
        {
            _mockUserManager.Setup(x => x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), GoodToken))
                .ReturnsAsync(true);

            var sut = CreateAccountController();
            var result = await sut.VerifyTwoFactorToken(new VerifyTwoFactorModel { UserId = UserId, Code = GoodToken});

            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), GoodToken), Times.Once);
            result.Should().BeOfType<VerifyTwoFactorModel>();
            result.Verified.Should().BeTrue();
        }
    }
}