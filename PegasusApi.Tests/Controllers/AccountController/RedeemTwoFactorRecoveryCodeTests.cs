using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using PegasusApi.Models.Account;

namespace PegasusApi.Tests.Controllers.AccountController
{
    public class RedeemTwoFactorRecoveryCodeTests : AccountControllerTestsBase
    {
        
        [Test]
        public async Task RedeemTwoFactorRecoveryCode_UserIsNull_ReturnsSucceededFalse()
        {
            MockUserManager.Setup(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()));

            var model = new RedeemTwoFactorRecoveryCodeModel { UserId = UnknownUserId };
            var sut = CreateAccountController();
            var result = await sut.RedeemTwoFactorRecoveryCode(model);

            MockUserManager.Verify(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<RedeemTwoFactorRecoveryCodeModel>();
            result.Succeeded.Should().BeFalse();
        }

        [Test]
        public async Task RedeemTwoFactorRecoveryCode_UserIdIsNull_ReturnsSucceededFalse()
        {
            var sut = CreateAccountController();
            var result = await sut.RedeemTwoFactorRecoveryCode(new RedeemTwoFactorRecoveryCodeModel());

            MockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<RedeemTwoFactorRecoveryCodeModel>();
            result.Succeeded.Should().BeFalse();
        }

        [Test]
        public async Task RedeemTwoFactorRecoveryCode_UserNotFound_ReturnsSucceededFalse()
        {
            MockUserManager.Setup(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()));

            var model = new RedeemTwoFactorRecoveryCodeModel { UserId = UnknownUserId };
            var sut = CreateAccountController();
            var result = await sut.RedeemTwoFactorRecoveryCode(model);

            MockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            MockUserManager.Verify(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<RedeemTwoFactorRecoveryCodeModel>();
            result.Succeeded.Should().BeFalse();
        }

        [Test]
        public async Task RedeemTwoFactorRecoveryCode_RedeemHasErrors_ReturnsSucceededFalse()
        {
            var errors = new[] { new IdentityError { Code = string.Empty, Description = string.Empty } };
            
            MockUserManager
                .Setup(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            var model = new RedeemTwoFactorRecoveryCodeModel { UserId = UserId };
            var sut = CreateAccountController();
            var result = await sut.RedeemTwoFactorRecoveryCode(model);

            MockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            MockUserManager.Verify(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            result.Should().BeOfType<RedeemTwoFactorRecoveryCodeModel>();
            result.Succeeded.Should().BeFalse();
        }

        [Test]
        public async Task RedeemTwoFactorRecoveryCode_RedeemSuccess_ReturnsSucceededTrue()
        {
            MockUserManager
                .Setup(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var model = new RedeemTwoFactorRecoveryCodeModel { UserId = UserId };
            var sut = CreateAccountController();
            var result = await sut.RedeemTwoFactorRecoveryCode(model);

            MockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            MockUserManager.Verify(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
            result.Should().BeOfType<RedeemTwoFactorRecoveryCodeModel>();
            result.Succeeded.Should().BeTrue();
        }

    }
}