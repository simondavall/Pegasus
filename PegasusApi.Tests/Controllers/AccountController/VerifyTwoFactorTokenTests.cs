using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using PegasusApi.Models.Account;

namespace PegasusApi.Tests.Controllers.AccountController
{
    public class VerifyTwoFactorTokenTests : AccountControllerTestsBase
    {
        private const string GoodToken = "good-token";
        private const string BadToken = "bad-token";
        
        [Test]
        public async Task VerifyTwoFactorToken_ModelIsMull_ReturnsEmptyModel()
        {
            var sut = CreateAccountController();
            var result = await sut.VerifyTwoFactorToken(null);
            
            MockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<VerifyTwoFactorModel>();
            result.UserId.Should().BeNull();
        }

        [Test]
        public async Task VerifyTwoFactorToken_UserIsMull_ReturnsNotVerified()
        {
            MockUserManager.Setup(x => x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var sut = CreateAccountController();
            var result = await sut.VerifyTwoFactorToken(new VerifyTwoFactorModel { UserId = UnknownUserId });

            MockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            MockUserManager.Verify(x => x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            result.Should().BeOfType<VerifyTwoFactorModel>();
            result.Verified.Should().BeFalse();
        }

        [Test]
        public async Task VerifyTwoFactorToken_NotVerified_ReturnsNotVerifiedIsFalse()
        {
            MockUserManager.Setup(x => x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), BadToken))
                .ReturnsAsync(false);

            var sut = CreateAccountController();
            var result = await sut.VerifyTwoFactorToken(new VerifyTwoFactorModel { UserId = UserId, Code = BadToken});

            MockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            MockUserManager.Verify(x => x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), BadToken), Times.Once);
            result.Should().BeOfType<VerifyTwoFactorModel>();
            result.Verified.Should().BeFalse();
        }

        [Test]
        public async Task VerifyTwoFactorToken_Verified_ReturnsNotVerifiedIsTrue()
        {
            MockUserManager.Setup(x => x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), GoodToken))
                .ReturnsAsync(true);

            var sut = CreateAccountController();
            var result = await sut.VerifyTwoFactorToken(new VerifyTwoFactorModel { UserId = UserId, Code = GoodToken});

            MockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            MockUserManager.Verify(x => x.VerifyTwoFactorTokenAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), GoodToken), Times.Once);
            result.Should().BeOfType<VerifyTwoFactorModel>();
            result.Verified.Should().BeTrue();
        }
    }
}