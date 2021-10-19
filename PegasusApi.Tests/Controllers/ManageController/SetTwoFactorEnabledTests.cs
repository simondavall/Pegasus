using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Tests.Controllers.ManageController
{
    public class SetTwoFactorEnabledTests : BaseControllerTest
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
        public async Task SetTwoFactorEnabled_ModelNullOrUserIdNullOrUserNotFound_ReturnsModelWithError()
        {
            var sut = CreateManageController();
            await CaseModelIsNull<SetTwoFactorEnabledModel, SetTwoFactorEnabledModel>(sut.SetTwoFactorEnabled);
            await CaseUserIdIsNull<SetTwoFactorEnabledModel, SetTwoFactorEnabledModel>(sut.SetTwoFactorEnabled);
            await CaseUserNotFound<SetTwoFactorEnabledModel, SetTwoFactorEnabledModel>(sut.SetTwoFactorEnabled);
        }
        
        [Test]
        public async Task SetTwoFactorEnabled_Failed2FaEnable_ReturnsModelWithErrors()
        {
            MockUserManager.Setup(x => x.SetTwoFactorEnabledAsync(It.IsAny<IdentityUser>(), It.IsAny<bool>()))
                .ReturnsAsync(IdentityResult.Failed(TestError));

            var sut = CreateManageController();
            var result = await sut.SetTwoFactorEnabled(new SetTwoFactorEnabledModel {UserId = UserId});

            VerifyErrorLogged(Times.Once());
            result.Should().BeOfType<SetTwoFactorEnabledModel>();
            result.Succeeded.Should().BeFalse();
            AssertHasErrors(result, 1);
        }
        
        [Test]
        public async Task SetTwoFactorEnabled_Success2FaEnable_ReturnsModelWithErrors()
        {
            MockUserManager.Setup(x => x.SetTwoFactorEnabledAsync(It.IsAny<IdentityUser>(), It.IsAny<bool>()))
                .ReturnsAsync(IdentityResult.Success);

            var sut = CreateManageController();
            var result = await sut.SetTwoFactorEnabled(new SetTwoFactorEnabledModel {UserId = UserId});

            VerifyErrorLogged(Times.Never());
            result.Should().BeOfType<SetTwoFactorEnabledModel>();
            result.Succeeded.Should().BeTrue();
            AssertHasErrors(result, 0);
        }
    }
}