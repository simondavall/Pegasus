using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Tests.Controllers.ManageController
{
    public class VerifyTwoFactorTokenTests : BaseControllerTest
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
        public async Task VerifyTwoFactorToken_ModelNullOrUserIdNullOrUserNotFound_ReturnsModelWithError()
        {
            var sut = CreateManageController();
            await CaseModelIsNull<VerifyTwoFactorTokenModel>(sut.VerifyTwoFactorToken);
            await CaseUserIdIsNull<VerifyTwoFactorTokenModel>(sut.VerifyTwoFactorToken);
            await CaseUserNotFound<VerifyTwoFactorTokenModel>(sut.VerifyTwoFactorToken);
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
    }
}