using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Tests.Controllers.ManageController
{
    public class TwoFactorAuthenticationTests : BaseControllerTest
    {
        private Mock<IUsersData> _mockUsersData;

        [SetUp]
        public void EachTestSetup()
        {
            _mockUsersData = new Mock<IUsersData>();
        }

        private PegasusApi.Controllers.ManageController CreateManageController()
        {
            return new PegasusApi.Controllers.ManageController(MockUserManager.Object, _mockUsersData.Object, null, null, MockLogger.Object);
        }
        
        [Test]
        public async Task TwoFactorAuthentication_UserIdIsNullOrNotFound_ReturnsModelWithError()
        {
            var sut = CreateManageController();
            await CaseUserIdIsNull(sut.TwoFactorAuthentication);
            await CaseUserNotFound(sut.TwoFactorAuthentication);
        }

        [Test]
        public async Task TwoFactorAuthentication_False_ReturnsModelHasPasswordFalse()
        {
            const int recoveryCodesLeft = 5;
            MockUserManager.Setup(x => x.GetAuthenticatorKeyAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync("auth-key");
            MockUserManager.Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(true);
            MockUserManager.Setup(x => x.CountRecoveryCodesAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(recoveryCodesLeft);
            
            var sut = CreateManageController();
            var result = await sut.TwoFactorAuthentication(UserId);

            VerifyErrorLogged(Times.Never());
            result.Should().BeOfType<TwoFactorAuthenticationModel>();
            result.HasAuthenticator.Should().BeTrue();
            result.Is2FaEnabled.Should().BeTrue();
            result.RecoveryCodesLeft.Should().Be(recoveryCodesLeft);
            AssertHasErrors(result, 0);
        }
    }
}