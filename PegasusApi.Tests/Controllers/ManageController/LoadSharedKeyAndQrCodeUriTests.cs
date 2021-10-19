using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.DataAccess;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Tests.Controllers.ManageController
{
    public class LoadSharedKeyAndQrCodeUriTests : BaseControllerTest
    {
        private Mock<IUsersData> _mockUsersData;
        private Mock<UrlEncoder> _mockUrlEncoder;
        private Mock<IConfiguration> _mockConfiguration;

        [SetUp]
        public void EachTestSetup()
        {
            _mockUsersData = new Mock<IUsersData>();
            _mockUrlEncoder = new Mock<UrlEncoder>();
            _mockConfiguration = new Mock<IConfiguration>();
        }

        private PegasusApi.Controllers.ManageController CreateManageController()
        {
            return new PegasusApi.Controllers.ManageController(MockUserManager.Object, _mockUsersData.Object, _mockUrlEncoder.Object, _mockConfiguration.Object, MockLogger.Object);
        }
        
        [Test]
        public async Task LoadSharedKeyAndQrCodeUri_UserIdIsNullOrNotFound_ReturnsModelWithError()
        {
            var sut = CreateManageController();
            await CaseUserIdIsNull(sut.LoadSharedKeyAndQrCodeUri);
            await CaseUserNotFound(sut.LoadSharedKeyAndQrCodeUri);
        }
        
        [Test]
        public async Task LoadSharedKeyAndQrCodeUri_FailToResetAuthenticatorKey_ReturnsModelWithErrors()
        {
            MockUserManager.Setup(x => x.GetAuthenticatorKeyAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(string.Empty);
            MockUserManager.Setup(x => x.ResetAuthenticatorKeyAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(IdentityResult.Failed(TestError));

            
            var sut = CreateManageController();
            var result = await sut.LoadSharedKeyAndQrCodeUri(UserId);

            VerifyErrorLogged(Times.Once());
            result.Should().BeOfType<AuthenticatorKeyModel>();
            result.SharedKey.Should().BeNull();
            AssertHasErrors(result, 1);
        }
        
        [Test]
        public async Task LoadSharedKeyAndQrCodeUri_AuthenticatorKeyStillNullAfterReset_ReturnsModelWithErrors()
        {
            MockUserManager.Setup(x => x.GetAuthenticatorKeyAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(string.Empty);
            MockUserManager.Setup(x => x.ResetAuthenticatorKeyAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(IdentityResult.Success);
            
            var sut = CreateManageController();
            var result = await sut.LoadSharedKeyAndQrCodeUri(UserId);

            VerifyErrorLogged(Times.Once());
            result.Should().BeOfType<AuthenticatorKeyModel>();
            result.SharedKey.Should().BeNull();
            AssertHasErrors(result, 1);
        }
        
        [Test]
        public async Task LoadSharedKeyAndQrCodeUri_AuthenticatorKeyRetrieved_ReturnsModel()
        {
            MockUserManager.SetupSequence(x => x.GetAuthenticatorKeyAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(string.Empty)
                .ReturnsAsync("1234567890");
            MockUserManager.Setup(x => x.ResetAuthenticatorKeyAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(IdentityResult.Success);
            MockUserManager.Setup(x => x.GetEmailAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(Username);
            _mockUrlEncoder.SetupSequence(x => x.Encode(It.IsAny<string>()))
                .Returns("encoded-project")
                .Returns("encoded-email");
            _mockConfiguration.Setup(x => x[It.IsAny<string>()]).Returns("project-name");
            
            var sut = CreateManageController();
            var result = await sut.LoadSharedKeyAndQrCodeUri(UserId);

            VerifyErrorLogged(Times.Never());
            result.Should().BeOfType<AuthenticatorKeyModel>();
            result.SharedKey.Should().Be("1234 5678 90");
            result.AuthenticatorUri.Should().Contain("encoded-project");
            result.AuthenticatorUri.Should().Contain("encoded-email");
            AssertHasErrors(result, 0);
        }
        
        
    }
}