using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Tests.Controllers.ManageController
{
    public class ChangePasswordTests : BaseControllerTest
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
        public async Task ChangePassword_ModelNullOrUserIdNullOrUserNotFound_ReturnsModelWithError()
        {
            var sut = CreateManageController();
            await CaseModelIsNull<ChangePasswordModel>(sut.ChangePassword);
            await CaseUserIdIsNull<ChangePasswordModel>(sut.ChangePassword);
            await CaseUserNotFound<ChangePasswordModel>(sut.ChangePassword);
        }

        [Test]
        public async Task ChangePassword_Failed_ReturnsModelWithError()
        {
            MockUserManager.Setup(x => x.ChangePasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(TestError));
            
            var sut = CreateManageController();
            var result = await sut.ChangePassword(new ChangePasswordModel {UserId = UserId});

            VerifyErrorLogged(Times.Once());
            result.Should().BeOfType<ChangePasswordModel>();
            result.Succeeded.Should().BeFalse();
            AssertHasErrors(result, 1);
        }
        
        [Test]
        public async Task ChangePassword_Success_ReturnsModelWithSucceededTrue()
        {
            MockUserManager.Setup(x => x.ChangePasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            
            var sut = CreateManageController();
            var result = await sut.ChangePassword(new ChangePasswordModel {UserId = UserId});

            VerifyErrorLogged(Times.Never());
            result.Should().BeOfType<ChangePasswordModel>();
            result.Succeeded.Should().BeTrue();
            AssertHasErrors(result, 0);
        }
    }
}