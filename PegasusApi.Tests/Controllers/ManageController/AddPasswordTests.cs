using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Tests.Controllers.ManageController
{
    public class AddPasswordTests : BaseControllerTest
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
        public async Task AddPassword_ModelNullOrUserIdNullOrUserNotFound_ReturnsModelWithError()
        {
            var sut = CreateManageController();
            await CaseModelIsNull<SetPasswordModel, SetPasswordModel>(sut.AddPassword);
            await CaseUserIdIsNull<SetPasswordModel, SetPasswordModel>(sut.AddPassword);
            await CaseUserNotFound<SetPasswordModel, SetPasswordModel>(sut.AddPassword);
        }
        
        [Test]
        public async Task AddPassword_Failed_ReturnsModelWithError()
        {
            MockUserManager.Setup(x => x.AddPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(TestError));
            
            var sut = CreateManageController();
            var result = await sut.AddPassword(new SetPasswordModel {UserId = UserId});

            VerifyErrorLogged(Times.Once());
            result.Should().BeOfType<SetPasswordModel>();
            result.Succeeded.Should().BeFalse();
            AssertHasErrors(result, 1);
        }
        
        [Test]
        public async Task AddPassword_Success_ReturnsModelWithSucceededTrue()
        {
            MockUserManager.Setup(x => x.AddPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            
            var sut = CreateManageController();
            var result = await sut.AddPassword(new SetPasswordModel {UserId = UserId});

            VerifyErrorLogged(Times.Never());
            result.Should().BeOfType<SetPasswordModel>();
            result.Succeeded.Should().BeTrue();
            AssertHasErrors(result, 0);
        }
    }
}