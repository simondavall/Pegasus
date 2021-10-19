using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Tests.Controllers.ManageController
{
    public class GetTwoFactorEnabledTests : BaseControllerTest
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
        public async Task GetTwoFactorEnabled_UserIdIsNullOrNotFound_ReturnsModelWithError()
        {
            var sut = CreateManageController();
            await CaseUserIdIsNull(sut.GetTwoFactorEnabled);
            await CaseUserNotFound(sut.GetTwoFactorEnabled);
        }
        
        [Test]
        public async Task GetTwoFactorEnabled_TwoFactorNotEnabled_ReturnsModelWitEnabledFalse()
        {
            MockUserManager.Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(false);
            
            var sut = CreateManageController();
            var result = await sut.GetTwoFactorEnabled(UserId);

            VerifyErrorLogged(Times.Never());
            result.Should().BeOfType<GetTwoFactorEnabledModel>();
            result.IsEnabled.Should().BeFalse();
            AssertHasErrors(result, 0);
        }
        
        [Test]
        public async Task GetTwoFactorEnabled_TwoFactorIsEnabled_ReturnsModelWitEnabledTrue()
        {
            MockUserManager.Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(true);
            
            var sut = CreateManageController();
            var result = await sut.GetTwoFactorEnabled(UserId);

            VerifyErrorLogged(Times.Never());
            result.Should().BeOfType<GetTwoFactorEnabledModel>();
            result.IsEnabled.Should().BeTrue();
            AssertHasErrors(result, 0);
        }
    }
}