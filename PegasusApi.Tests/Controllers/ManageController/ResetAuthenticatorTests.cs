using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Tests.Controllers.ManageController
{
    public class ResetAuthenticatorTests : BaseControllerTest
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
        public async Task ResetAuthenticator_ModelNullOrUserIdNullOrUserNotFound_ReturnsModelWithError()
        {
            var sut = CreateManageController();
            await CaseModelIsNull<ResetAuthenticatorModel>(sut.ResetAuthenticator);
            await CaseUserIdIsNull<ResetAuthenticatorModel>(sut.ResetAuthenticator);
            await CaseUserNotFound<ResetAuthenticatorModel>(sut.ResetAuthenticator);
        }
        
        [Test]
        public async Task ResetAuthenticator_Failed2FaEnable_ReturnsModelWithErrors()
        {
            MockUserManager.Setup(x => x.SetTwoFactorEnabledAsync(It.IsAny<IdentityUser>(), false))
                .ReturnsAsync(IdentityResult.Failed(TestError));

            var sut = CreateManageController();
            var result = await sut.ResetAuthenticator(new ResetAuthenticatorModel {UserId = UserId});

            VerifyErrorLogged(Times.Once());
            result.Should().BeOfType<ResetAuthenticatorModel>();
            AssertHasErrors(result, 1);
        }
        
        [Test]
        public async Task ResetAuthenticator_FailedResetAuthKey_ReturnsModelWithErrors()
        {
            MockUserManager.Setup(x => x.SetTwoFactorEnabledAsync(It.IsAny<IdentityUser>(), false))
                .ReturnsAsync(IdentityResult.Success);
            MockUserManager.Setup(x => x.ResetAuthenticatorKeyAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(IdentityResult.Failed(TestError));

            var sut = CreateManageController();
            var result = await sut.ResetAuthenticator(new ResetAuthenticatorModel {UserId = UserId});

            VerifyErrorLogged(Times.Once());
            result.Should().BeOfType<ResetAuthenticatorModel>();
            AssertHasErrors(result, 1);
        }
        
        [Test]
        public async Task ResetAuthenticator_Success_ReturnsModelWithNoErrors()
        {
            MockUserManager.Setup(x => x.SetTwoFactorEnabledAsync(It.IsAny<IdentityUser>(), false))
                .ReturnsAsync(IdentityResult.Success);
            MockUserManager.Setup(x => x.ResetAuthenticatorKeyAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var sut = CreateManageController();
            var result = await sut.ResetAuthenticator(new ResetAuthenticatorModel {UserId = UserId});

            VerifyErrorLogged(Times.Never());
            result.Should().BeOfType<ResetAuthenticatorModel>();
            AssertHasErrors(result, 0);
        }
    }
}