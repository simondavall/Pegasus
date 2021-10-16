using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Library.JwtAuthentication.Models;
using Pegasus.Library.Models.Manage;
using ManageControllerStrings = Pegasus.Library.Services.Resources.Resources.ControllerStrings.ManageController;


namespace PegasusTests.Controllers.ManageControllerTests
{
    internal class ChangePasswordTests : ManageControllerTestsBase
    {
        [Test]
        public async Task GET_ChangePassword_HasErrors_ReturnsViewResult()
        {
            MockManageEndpoint.Setup(x => x.HasPasswordAsync(It.IsAny<string>()))
                .ReturnsAsync(new HasPasswordModel { Errors = TestErrors, StatusMessage = "Error" });

            var sut = CreateManageController();
            var result = await sut.ChangePassword();

            result.Should().BeOfType<ViewResult>();
            sut.ModelState?.ErrorCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task GET_ChangePassword_HasNoPassword_RedirectsToSetPassword()
        {
            MockManageEndpoint.Setup(x => x.HasPasswordAsync(It.IsAny<string>()))
                .ReturnsAsync(new HasPasswordModel { StatusMessage = "OK", HasPassword = false });

            var sut = CreateManageController();
            var result = await sut.ChangePassword();

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = (RedirectToActionResult)result;
            redirectToActionResult.ActionName.Should().Be(nameof(sut.SetPassword));
        }

        [Test]
        public async Task GET_ChangePassword_HasPassword_ReturnsViewResult()
        {
            MockManageEndpoint.Setup(x => x.HasPasswordAsync(It.IsAny<string>()))
                .ReturnsAsync(new HasPasswordModel { StatusMessage = "OK", HasPassword = true });

            var sut = CreateManageController();
            var result = await sut.ChangePassword();

            result.Should().BeOfType<ViewResult>();
        }

        [Test]
        public async Task POST_ChangePassword_InvalidModelState_ReturnsViewResult()
        {
            MockManageEndpoint.Setup(x => x.HasPasswordAsync(It.IsAny<string>()));

            var sut = CreateManageController();
            sut.ModelState.AddModelError("Error", "An error occurred.");
            var result = await sut.ChangePassword(new ChangePasswordModel());

            result.Should().BeOfType<ViewResult>();
        }

        [Test]
        public async Task POST_ChangePassword_HasErrors_ReturnsViewResultWithErrorInModelState()
        {
            MockManageEndpoint.Setup(x => x.ChangePasswordAsync(It.IsAny<ChangePasswordModel>()))
                .ReturnsAsync(new ChangePasswordModel
                    { Succeeded = false, Errors = TestErrors, StatusMessage = "Error" });

            var sut = CreateManageController();
            var result = await sut.ChangePassword(new ChangePasswordModel());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<ChangePasswordModel>();
            sut.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public async Task POST_ChangePassword_NoErrors_ReturnsViewResultWithErrorInModelState()
        {
            var authResult = GetAuthenticateResult();
            MockHttpContextWrapper.Setup(x => x.AuthenticateAsync(It.IsAny<string>()))
                .ReturnsAsync(authResult);
            MockAuthenticationEndpoint.Setup(x => x.Authenticate2Fa(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatedUser { AccessToken = "access-token", Authenticated = true });
            MockTokenAccessor.Setup(x => x.GetAccessTokenWithClaimsPrincipal(It.IsAny<AuthenticatedUser>()))
                .Returns(new TokenWithClaimsPrincipal());
            MockManageEndpoint.Setup(x => x.ChangePasswordAsync(It.IsAny<ChangePasswordModel>()))
                .ReturnsAsync(new ChangePasswordModel { Succeeded = true, StatusMessage = "OK" });

            var sut = CreateManageController();
            var result = await sut.ChangePassword(new ChangePasswordModel());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<ChangePasswordModel>();
            var statusMessage = ((ChangePasswordModel)((ViewResult)result).Model).StatusMessage;
            statusMessage.Should().Be(ManageControllerStrings.PasswordChangedSuccess);
        }
    }
}