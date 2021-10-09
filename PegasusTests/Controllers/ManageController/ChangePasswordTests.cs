using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Library.JwtAuthentication.Models;
using Pegasus.Library.Models.Manage;
using Pegasus.Library.Services.Resources;

namespace PegasusTests.Controllers.ManageController
{
    class ChangePasswordTests : ManageControllerTestsBase
    {
        [Test]
        public async Task GET_ChangePassword_HasErrors_ReturnsViewResult()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()))
                .ReturnsAsync(new HasPasswordModel {Errors = new List<IdentityError> {new IdentityError {Description = "Error Message"}}, StatusMessage = "Error"});

            var sut = CreateManageController();
            var result = await sut.ChangePassword();

            _mockApiHelper.Verify(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task GET_ChangePassword_HasNoPassword_RedirectsToSetPassword()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()))
                .ReturnsAsync(new HasPasswordModel {StatusMessage = "OK", HasPassword = false});
            
            var sut = CreateManageController();
            var result = await sut.ChangePassword();

            _mockApiHelper.Verify(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(nameof(sut.SetPassword),((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task GET_ChangePassword_HasPassword_ReturnsViewResult()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()))
                .ReturnsAsync(new HasPasswordModel {StatusMessage = "OK", HasPassword = true});
            
            var sut = CreateManageController();
            var result = await sut.ChangePassword();

            _mockApiHelper.Verify(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task POST_ChangePassword_InvalidModelState_ReturnsViewResult()
        {
            var sut = CreateManageController();
            sut.ModelState.AddModelError("Error", "An error occurred.");
            var result = await sut.ChangePassword(new ChangePasswordModel());

            _mockApiHelper.Verify(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()), Times.Exactly(0));
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task POST_ChangePassword_HasErrors_ReturnsViewResultWithErrorInModelState()
        {
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<ChangePasswordModel>(), It.IsAny<string>()))
                .ReturnsAsync(new ChangePasswordModel {Succeeded = false, Errors = new List<IdentityError> {new IdentityError {Description = "Error Message"}}, StatusMessage = "Error"});
            
            var sut = CreateManageController();
            var result = await sut.ChangePassword(new ChangePasswordModel());

            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<ChangePasswordModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<ChangePasswordModel>(((ViewResult) result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "An error occurred.");
        }

        [Test]
        public async Task POST_ChangePassword_NoErrors_ReturnsViewResultWithErrorInModelState()
        {
            var authResult = GetAuthenticateResult();
            _mockHttpContextWrapper.Setup(x => x.AuthenticateAsync(It.IsAny<string>())).ReturnsAsync(authResult);

            _mockAuthenticationEndpoint.Setup(x => x.Authenticate2Fa(It.IsAny<string>())).ReturnsAsync(new AuthenticatedUser {AccessToken = "access-token", Authenticated = true});

            _mockTokenAccessor.Setup(x => x.GetAccessTokenWithClaimsPrincipal(It.IsAny<AuthenticatedUser>())).Returns(new TokenWithClaimsPrincipal());

            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<ChangePasswordModel>(), It.IsAny<string>()))
                .ReturnsAsync(new ChangePasswordModel {Succeeded = true, StatusMessage = "OK"});
            
            var sut = CreateManageController();
            var result = await sut.ChangePassword(new ChangePasswordModel());

            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<ChangePasswordModel>(), It.IsAny<string>()), Times.Exactly(1));
            _mockHttpContextWrapper.Verify(x => x.SignOutAsync(), Times.Exactly(1));
            _mockHttpContextWrapper.Verify(x => x.SignInAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<ChangePasswordModel>(((ViewResult) result).Model);
            Assert.AreEqual(Resources.ControllerStrings.ManageController.PasswordChangedSuccess, ((ChangePasswordModel)((ViewResult)result).Model).StatusMessage);
        }


    }
}
