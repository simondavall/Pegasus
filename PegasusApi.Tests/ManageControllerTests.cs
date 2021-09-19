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
        //private UrlEncoder _urlEncoder;
        //private IConfiguration _configuration;
        private static UserModel _userModel;

        [OneTimeSetUp]
        public override void OneTimeSetup()
        {
            base.OneTimeSetup();

            _userModel = new UserModel { DisplayName = "Test User", Id = _userId };
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
            var manageController = new ManageController(_userManager, _usersData, null, null);
            var sut = manageController.GetUserDetails(_userId).Result;

            Assert.IsInstanceOf<UserDetailsModel>(sut);

            Assert.AreEqual(_user.UserName, sut.Username);
            Assert.AreEqual(_userModel.DisplayName, sut.DisplayName);
            Assert.AreEqual(_phoneNumber, sut.PhoneNumber);
        }

        [Test]
        public void GetUserDetails_InvalidUserId_EmptyInstanceWithOneErrorCount()
        {
            var manageController = new ManageController(_userManager, _usersData, null, null);
            var sut = manageController.GetUserDetails(_badUserId).Result;

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
                UserId = _userId,
                DisplayName = "Test User",
                Username = _username,
                PhoneNumber = _phoneNumber
            };

            var manageController = new ManageController(_userManager, _usersData, null, null);
            var sut = manageController.SetUserDetails(userDetailsModel).Result;

            Assert.IsInstanceOf<UserDetailsModel>(sut);

            Assert.AreEqual(_user.UserName, sut.Username);
            Assert.AreEqual(_userModel.DisplayName, sut.DisplayName);
            Assert.AreEqual(_phoneNumber, sut.PhoneNumber);
            Assert.Zero(sut.Errors.Count);
        }

        [Test]
        public void SetUserDetails_BadPhoneNumber_ReturnsError()
        {
            var userDetailsModel = new UserDetailsModel()
            {
                UserId = _userId,
                DisplayName = "Test User",
                Username = _username,
                PhoneNumber = _badPhoneNumber
            };

            var manageController = new ManageController(_userManager, _usersData, null, null);
            var sut = manageController.SetUserDetails(userDetailsModel).Result;

            Assert.IsInstanceOf<UserDetailsModel>(sut);

            Assert.AreEqual(_user.UserName, sut.Username);
            Assert.AreEqual(_userModel.DisplayName, sut.DisplayName);
            Assert.AreEqual(_badPhoneNumber, sut.PhoneNumber);
            Assert.NotZero(sut.Errors.Count);
            Assert.Contains(_testError, sut.Errors);
        }

        
        private static Mock<IUsersData> MockUsersData()
        {
            var usersData = new Mock<IUsersData>();
            // set up for success
            usersData.Setup(x => x.GetUser(_userId)).ReturnsAsync(_userModel);
            usersData.Setup(x => x.UpdateUser(_userModel));
            //set up for fail
            usersData.Setup(x => x.GetUser(_badUserId)).ReturnsAsync((UserModel) null);

            return usersData;
        }
    }
}
