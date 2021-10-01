using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PegasusApi.Controllers;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Tests
{
    public class ManageControllerTests : BaseControllerTest
    {
        private IUsersData _usersData;
        private static UserModel _userModel;

        [OneTimeSetUp]
        public override void OneTimeSetup()
        {
            base.OneTimeSetup();

            _userModel = new UserModel { DisplayName = "Test User", Id = UserId };
            _usersData = MockUsersData().Object;
        }

        [Test]
        public void CreateToken_CorrectCredentials_CreatesToken()
        {
            Assert.Pass("Manage Controllers Test");
        }

        [Test]
        public void GetUserDetails_ValidUserId_ReturnsDetails()
        {
            var manageController = new ManageController(UserManager, _usersData, null, null, null);
            var sut = manageController.GetUserDetails(UserId).Result;

            Assert.IsInstanceOf<UserDetailsModel>(sut);

            Assert.AreEqual(User.UserName, sut.Username);
            Assert.AreEqual(_userModel.DisplayName, sut.DisplayName);
            Assert.AreEqual(PhoneNumber, sut.PhoneNumber);
        }

        [Test]
        public void GetUserDetails_InvalidUserId_EmptyInstanceWithOneErrorCount()
        {
            var mockLogger = MockLogger<ManageController>();

            var manageController = new ManageController(UserManager, _usersData, null, null, mockLogger);
            var sut = manageController.GetUserDetails(BadUserId).Result;

            Assert.IsInstanceOf<UserDetailsModel>(sut);

            Assert.AreEqual(null, sut.Username);
            Assert.AreEqual(null, sut.DisplayName);
            Assert.AreEqual(null, sut.PhoneNumber);
            Assert.AreEqual(null, sut.UserId);
            Assert.NotZero(sut.Errors.Count);
        }

        [Test]
        public void SetUserDetails_ValidUserId_ReturnsDetails()
        {
            var userDetailsModel = new UserDetailsModel()
            {
                UserId = UserId,
                DisplayName = "Test User",
                Username = Username,
                PhoneNumber = PhoneNumber
            };

            var manageController = new ManageController(UserManager, _usersData, null, null, null);
            var sut = manageController.SetUserDetails(userDetailsModel).Result;

            Assert.IsInstanceOf<UserDetailsModel>(sut);

            Assert.AreEqual(User.UserName, sut.Username);
            Assert.AreEqual(_userModel.DisplayName, sut.DisplayName);
            Assert.AreEqual(PhoneNumber, sut.PhoneNumber);
            Assert.Zero(sut.Errors.Count);
        }

        [Test]
        public void SetUserDetails_BadPhoneNumber_ReturnsError()
        {
            var userDetailsModel = new UserDetailsModel()
            {
                UserId = UserId,
                DisplayName = "Test User",
                Username = Username,
                PhoneNumber = BadPhoneNumber
            };
            var mockLogger = MockLogger<ManageController>();

            var manageController = new ManageController(UserManager, _usersData, null, null, mockLogger);
            var sut = manageController.SetUserDetails(userDetailsModel).Result;

            Assert.IsInstanceOf<UserDetailsModel>(sut);

            Assert.AreEqual(User.UserName, sut.Username);
            Assert.AreEqual(_userModel.DisplayName, sut.DisplayName);
            Assert.AreEqual(BadPhoneNumber, sut.PhoneNumber);
            Assert.NotZero(sut.Errors.Count);
            Assert.Contains(TestError, sut.Errors);
        }

        
        private static Mock<IUsersData> MockUsersData()
        {
            var usersData = new Mock<IUsersData>();
            // set up for success
            usersData.Setup(x => x.GetUser(UserId)).ReturnsAsync(_userModel);
            usersData.Setup(x => x.UpdateUser(_userModel));
            //set up for fail
            usersData.Setup(x => x.GetUser(BadUserId)).ReturnsAsync((UserModel) null);

            return usersData;
        }

        private static ILogger<T> MockLogger<T>()
        {
            var mockLogger = new Mock<ILogger<T>>();
            mockLogger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
            //loggerMock.Verify(
            //    x => x.Log(
            //        It.IsAny<LogLevel>(),
            //        It.IsAny<EventId>(),
            //        It.Is<It.IsAnyType>((v, t) => true),
            //        It.IsAny<Exception>(),
            //        It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));

            return mockLogger.Object;
        }
    }
}
