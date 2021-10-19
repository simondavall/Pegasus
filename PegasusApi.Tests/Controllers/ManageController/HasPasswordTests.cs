using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Tests.Controllers.ManageController
{
    public class HasPasswordTests : BaseControllerTest
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
        public async Task HasPassword_UserIdIsNullOrNotFound_ReturnsModelWithError()
        {
            var sut = CreateManageController();
            await CaseUserIdIsNull(sut.HasPassword);
            await CaseUserNotFound(sut.HasPassword);
        }

        [Test]
        public async Task HasPassword_False_ReturnsModelHasPasswordFalse()
        {
            MockUserManager.Setup(x => x.HasPasswordAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(false);
            
            var sut = CreateManageController();
            var result = await sut.HasPassword(UserId);

            VerifyErrorLogged(Times.Never());
            result.Should().BeOfType<HasPasswordModel>();
            result.HasPassword.Should().BeFalse();
            AssertHasErrors(result, 0);
        }
        
        [Test]
        public async Task HasPassword_True_ReturnsModelHasPasswordTrue()
        {
            MockUserManager.Setup(x => x.HasPasswordAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(true);
            
            var sut = CreateManageController();
            var result = await sut.HasPassword(UserId);

            VerifyErrorLogged(Times.Never());
            result.Should().BeOfType<HasPasswordModel>();
            result.HasPassword.Should().BeTrue();
            AssertHasErrors(result, 0);
        }
    }
}