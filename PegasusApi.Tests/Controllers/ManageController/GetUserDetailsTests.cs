using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Tests.Controllers.ManageController
{
    public class GetUserDetailsTests : BaseControllerTest
    {
        private Mock<IUsersData> _mockUsersData;
        private UserModel _userModel;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _userModel = new UserModel { DisplayName = "Test User", Id = UserId };
        }
        
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
        public async Task GetUserDetails_UserIdIsNullOrNotFound_ReturnsModelWithError()
        {
            var sut = CreateManageController();
            await CaseUserIdIsNull(sut.GetUserDetails);
            await CaseUserNotFound(sut.GetUserDetails);
        }
        
        [Test]
        public async Task GetUserDetails_Success_ReturnsModel()
        {
            _mockUsersData.Setup(x => x.GetUser(UserId)).ReturnsAsync(_userModel);
            MockUserManager.Setup(x => x.GetPhoneNumberAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(PhoneNumber);
            
            var sut = CreateManageController();
            var result = await sut.GetUserDetails(UserId);

            VerifyErrorLogged(Times.Never());
            result.Should().BeOfType<UserDetailsModel>();
            result.DisplayName.Should().Be(_userModel.DisplayName);
            result.UserId.Should().Be(UserId);
            result.Username.Should().Be(Username);
            result.PhoneNumber.Should().Be(PhoneNumber);
            AssertHasErrors(result, 0);
        }
    }
}