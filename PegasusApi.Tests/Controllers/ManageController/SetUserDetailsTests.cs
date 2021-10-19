using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Tests.Controllers.ManageController
{
    public class SetUserDetailsTests : BaseControllerTest
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
        public async Task SetUserDetails_ModelNullOrUserIdNullOrUserNotFound_ReturnsModelWithError()
        {
            var sut = CreateManageController();
            await CaseModelIsNull<UserDetailsModel>(sut.SetUserDetails);
            await CaseUserIdIsNull<UserDetailsModel>(sut.SetUserDetails);
            await CaseUserNotFound<UserDetailsModel>(sut.SetUserDetails);
        }
        
        [Test]
        public async Task SetUserDetails_BadPhoneNumber_ReturnsModelWithErrors()
        {
            MockUserManager.Setup(x => x.SetPhoneNumberAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(TestError));
            
            var userDetailsModel = new UserDetailsModel()
            {
                UserId = UserId,
                DisplayName = "Test User",
                Username = Username,
                PhoneNumber = BadPhoneNumber
            };
            var sut = CreateManageController();
            var result = await sut.SetUserDetails(userDetailsModel);

            VerifyErrorLogged(Times.Once());
            AssertHasErrors(result, 1);
            result.Should().BeOfType<UserDetailsModel>();
            result.DisplayName.Should().Be(userDetailsModel.DisplayName);
            result.UserId.Should().Be(userDetailsModel.UserId);
            result.Username.Should().Be(userDetailsModel.Username);
            result.PhoneNumber.Should().Be(userDetailsModel.PhoneNumber);
            result.Errors.Should().Contain(TestError);
        }
        
        [Test]
        public async Task SetUserDetails_UpdateThrowsException_ReturnsModelWithErrors()
        {
            MockUserManager.Setup(x => x.SetPhoneNumberAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUsersData.Setup(x => x.UpdateUser(It.IsAny<UserModel>()))
                .Throws(new Exception("Test exception"));
            
            var userDetailsModel = new UserDetailsModel
            {
                UserId = UserId,
                DisplayName = "Test User",
                Username = Username,
                PhoneNumber = PhoneNumber
            };
            var sut = CreateManageController();
            var result = await sut.SetUserDetails(userDetailsModel);

            VerifyErrorLogged(Times.Once());
            AssertHasErrors(result, 1);
            result.Should().BeOfType<UserDetailsModel>();
            result.DisplayName.Should().Be(userDetailsModel.DisplayName);
            result.UserId.Should().Be(userDetailsModel.UserId);
            result.Username.Should().Be(userDetailsModel.Username);
            result.PhoneNumber.Should().Be(userDetailsModel.PhoneNumber);
        }
        
        [Test]
        public async Task SetUserDetails_UpdateSuccess_Returns()
        {
            MockUserManager.Setup(x => x.SetPhoneNumberAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUsersData.Setup(x => x.UpdateUser(It.IsAny<UserModel>()));

            var userDetailsModel = new UserDetailsModel
            {
                UserId = UserId,
                DisplayName = "Test User",
                Username = Username,
                PhoneNumber = PhoneNumber
            };
            var sut = CreateManageController();
            var result = await sut.SetUserDetails(userDetailsModel);

            VerifyErrorLogged(Times.Never());
            AssertHasErrors(result, 0);
            result.Should().BeOfType<UserDetailsModel>();
            result.DisplayName.Should().Be(userDetailsModel.DisplayName);
            result.UserId.Should().Be(userDetailsModel.UserId);
            result.Username.Should().Be(userDetailsModel.Username);
            result.PhoneNumber.Should().Be(userDetailsModel.PhoneNumber);
        }
        
        
    }
}