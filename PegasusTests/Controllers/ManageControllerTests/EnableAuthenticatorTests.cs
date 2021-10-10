using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Library.Models.Manage;
using Pegasus.Library.Services.Resources;

namespace PegasusTests.Controllers.ManageControllerTests
{
    class EnableAuthenticatorTests : ManageControllerTestsBase
    {
        [Test]
        public async Task GET_EnableAuthenticator_HasErrors_ReturnsViewResultWithErrorInModelState()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<AuthenticatorKeyModel>(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatorKeyModel {Errors = TestErrors, StatusMessage = "Error"});
            
            var sut = CreateManageController();

            var result = await sut.EnableAuthenticator();

            _mockApiHelper.Verify(x => x.GetFromUri<AuthenticatorKeyModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<EnableAuthenticatorModel>(((ViewResult) result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task GET_EnableAuthenticator_NoErrors_ReturnsViewResult()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<AuthenticatorKeyModel>(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatorKeyModel {StatusMessage = "OK"});
            
            var sut = CreateManageController();
            var result = await sut.EnableAuthenticator();

            _mockApiHelper.Verify(x => x.GetFromUri<AuthenticatorKeyModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<EnableAuthenticatorModel>(((ViewResult) result).Model);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_EnableAuthenticator_InvalidModelState_ReturnsViewResultWithErrorInModelState()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<AuthenticatorKeyModel>(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatorKeyModel {StatusMessage = "OK"});
            
            var sut = CreateManageController();
            sut.ModelState.AddModelError(string.Empty, "Test error.");
            var result = await sut.EnableAuthenticator();

            _mockApiHelper.Verify(x => x.GetFromUri<AuthenticatorKeyModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<EnableAuthenticatorModel>(((ViewResult) result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_EnableAuthenticator_VerifyTokenHasErrors_ReturnsViewResultWithErrorInModelState()
        {
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<VerifyTwoFactorTokenModel>(), It.IsAny<string>()))
                .ReturnsAsync(new VerifyTwoFactorTokenModel {Errors = TestErrors, StatusMessage = "Error"});
            
            var sut = CreateManageController();
            var result = await sut.EnableAuthenticator(new EnableAuthenticatorModel());

            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<VerifyTwoFactorTokenModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<EnableAuthenticatorModel>(((ViewResult) result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "An error occurred.");
        }

        [Test]
        public async Task POST_EnableAuthenticator_InvalidToken_ReturnsViewResultWithErrorInModelState()
        {
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<VerifyTwoFactorTokenModel>(), It.IsAny<string>()))
                .ReturnsAsync(new VerifyTwoFactorTokenModel {IsTokenValid = false, StatusMessage = "Error"});
            _mockApiHelper.Setup(x => x.GetFromUri<AuthenticatorKeyModel>(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatorKeyModel {StatusMessage = "OK"});
            
            var sut = CreateManageController();
            var result = await sut.EnableAuthenticator(new EnableAuthenticatorModel());

            _mockApiHelper.Verify(x => x.GetFromUri<AuthenticatorKeyModel>(It.IsAny<string>()), Times.Exactly(1));
            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<VerifyTwoFactorTokenModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<EnableAuthenticatorModel>(((ViewResult) result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_EnableAuthenticator_SetTwoFactorEnabledHasErrors_ReturnsViewResultWithErrorInModelState()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<AuthenticatorKeyModel>(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatorKeyModel {StatusMessage = "OK"});
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<VerifyTwoFactorTokenModel>(), It.IsAny<string>()))
                .ReturnsAsync(new VerifyTwoFactorTokenModel {IsTokenValid = true, StatusMessage = "OK"});
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel {Errors = TestErrors, StatusMessage = "Error"});

            SetupSignInMocks();
            
            var sut = CreateManageController();
            var result = await sut.EnableAuthenticator(new EnableAuthenticatorModel());

            _mockApiHelper.Verify(x => x.GetFromUri<AuthenticatorKeyModel>(It.IsAny<string>()), Times.Never);
            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<VerifyTwoFactorTokenModel>(), It.IsAny<string>()), Times.Exactly(1));
            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<EnableAuthenticatorModel>(((ViewResult) result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_EnableAuthenticator_CheckRecoveryCodesStatusHasErrors_ReturnsViewResultWithErrorInModelState()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<AuthenticatorKeyModel>(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatorKeyModel {StatusMessage = "OK"});
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<VerifyTwoFactorTokenModel>(), It.IsAny<string>()))
                .ReturnsAsync(new VerifyTwoFactorTokenModel {IsTokenValid = true, StatusMessage = "OK"});
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel {StatusMessage = "OK"});
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<RecoveryCodeStatusModel>(), It.IsAny<string>()))
                .ReturnsAsync(new RecoveryCodeStatusModel {Errors = TestErrors, StatusMessage = "Error"});

            SetupSignInMocks();
            
            var sut = CreateManageController();
            var result = await sut.EnableAuthenticator(new EnableAuthenticatorModel());

            _mockApiHelper.Verify(x => x.GetFromUri<AuthenticatorKeyModel>(It.IsAny<string>()), Times.Never);
            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<VerifyTwoFactorTokenModel>(), It.IsAny<string>()), Times.Exactly(1));
            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()), Times.Exactly(1));
            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<RecoveryCodeStatusModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<EnableAuthenticatorModel>(((ViewResult) result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_EnableAuthenticator_RecoveryCodesHaveUpdates_RedirectsToShowRecoveryCodes()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<AuthenticatorKeyModel>(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatorKeyModel {StatusMessage = "OK"});
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<VerifyTwoFactorTokenModel>(), It.IsAny<string>()))
                .ReturnsAsync(new VerifyTwoFactorTokenModel {IsTokenValid = true, StatusMessage = "OK"});
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel {StatusMessage = "OK"});
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<RecoveryCodeStatusModel>(), It.IsAny<string>()))
                .ReturnsAsync(new RecoveryCodeStatusModel { IsUpdated = true, StatusMessage = "OK"});

            SetupSignInMocks();
            
            var sut = CreateManageController();
            var result = await sut.EnableAuthenticator(new EnableAuthenticatorModel());

            _mockApiHelper.Verify(x => x.GetFromUri<AuthenticatorKeyModel>(It.IsAny<string>()), Times.Never);
            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<VerifyTwoFactorTokenModel>(), It.IsAny<string>()), Times.Exactly(1));
            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()), Times.Exactly(1));
            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<RecoveryCodeStatusModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var routeValues = ((RedirectToActionResult)result).RouteValues;
            Assert.NotZero(routeValues.Count);
            Assert.AreEqual(Resources.ControllerStrings.ManageController.AuthenticatorAppVerified, routeValues["StatusMessage"]);
            Assert.AreEqual( "ShowRecoveryCodes", ((RedirectToActionResult) result).ActionName);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_EnableAuthenticator_NoRecoveryCodeUpdate_RedirectsToTwoFactorAuthentication()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<AuthenticatorKeyModel>(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatorKeyModel {StatusMessage = "OK"});
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<VerifyTwoFactorTokenModel>(), It.IsAny<string>()))
                .ReturnsAsync(new VerifyTwoFactorTokenModel {IsTokenValid = true, StatusMessage = "OK"});
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel {StatusMessage = "OK"});
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<RecoveryCodeStatusModel>(), It.IsAny<string>()))
                .ReturnsAsync(new RecoveryCodeStatusModel { IsUpdated = false, StatusMessage = "OK"});

            SetupSignInMocks();
            
            var sut = CreateManageController();
            var result = await sut.EnableAuthenticator(new EnableAuthenticatorModel());

            _mockApiHelper.Verify(x => x.GetFromUri<AuthenticatorKeyModel>(It.IsAny<string>()), Times.Never);
            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<VerifyTwoFactorTokenModel>(), It.IsAny<string>()), Times.Exactly(1));
            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()), Times.Exactly(1));
            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<RecoveryCodeStatusModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual( "TwoFactorAuthentication", ((RedirectToActionResult) result).ActionName);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed.");
        }
    }
}
