using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Library.Models.Manage;
using ManageControllerStrings = Pegasus.Library.Services.Resources.Resources.ControllerStrings.ManageController;

namespace PegasusTests.Controllers.ManageControllerTests
{
    internal class EnableAuthenticatorTests : ManageControllerTestsBase
    {
        [Test]
        public async Task GET_EnableAuthenticator_HasErrors_ReturnsViewResultWithErrorInModelState()
        {
            MockManageEndpoint.Setup(x => x.LoadSharedKeyAndQrCodeUriAsync(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatorKeyModel { Errors = TestErrors, StatusMessage = "Error" });

            var sut = CreateManageController();
            var result = await sut.EnableAuthenticator();

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<EnableAuthenticatorModel>();
            sut.ModelState.ErrorCount.Should().Be(2);
        }

        [Test]
        public async Task GET_EnableAuthenticator_NoErrors_ReturnsViewResult()
        {
            MockManageEndpoint.Setup(x => x.LoadSharedKeyAndQrCodeUriAsync(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatorKeyModel());

            var sut = CreateManageController();
            var result = await sut.EnableAuthenticator();

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<EnableAuthenticatorModel>();
            sut.ModelState.ErrorCount.Should().Be(0);
        }

        [Test]
        public async Task POST_EnableAuthenticator_InvalidModelState_ReturnsViewResultWithErrorInModelState()
        {
            MockManageEndpoint.Setup(x => x.LoadSharedKeyAndQrCodeUriAsync(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatorKeyModel());

            var sut = CreateManageController();
            sut.ModelState.AddModelError(string.Empty, "Test error.");
            var result = await sut.EnableAuthenticator(new EnableAuthenticatorModel());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<EnableAuthenticatorModel>();
            sut.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public async Task POST_EnableAuthenticator_VerifyTokenHasErrors_ReturnsViewResultWithErrorInModelState()
        {
            MockManageEndpoint.Setup(x => x.VerifyTwoFactorTokenAsync(It.IsAny<VerifyTwoFactorTokenModel>()))
                .ReturnsAsync(new VerifyTwoFactorTokenModel { IsTokenValid = false, Errors = TestErrors });

            var sut = CreateManageController();
            var result = await sut.EnableAuthenticator(new EnableAuthenticatorModel());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<EnableAuthenticatorModel>();
            sut.ModelState.ErrorCount.Should().Be(2);
        }

        [Test]
        public async Task POST_EnableAuthenticator_InvalidToken_ReturnsViewResultWithErrorInModelState()
        {
            MockManageEndpoint.Setup(x => x.LoadSharedKeyAndQrCodeUriAsync(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatorKeyModel());
            MockManageEndpoint.Setup(x => x.VerifyTwoFactorTokenAsync(It.IsAny<VerifyTwoFactorTokenModel>()))
                .ReturnsAsync(new VerifyTwoFactorTokenModel { IsTokenValid = false });

            var sut = CreateManageController();
            var result = await sut.EnableAuthenticator(new EnableAuthenticatorModel());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<EnableAuthenticatorModel>();
            sut.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public async Task POST_EnableAuthenticator_SetTwoFactorEnabledHasErrors_ReturnsViewResultWithErrorInModelState()
        {
            MockManageEndpoint.Setup(x => x.LoadSharedKeyAndQrCodeUriAsync(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatorKeyModel());
            MockManageEndpoint.Setup(x => x.VerifyTwoFactorTokenAsync(It.IsAny<VerifyTwoFactorTokenModel>()))
                .ReturnsAsync(new VerifyTwoFactorTokenModel { IsTokenValid = true });
            MockManageEndpoint.Setup(x => x.SetTwoFactorEnabledAsync(It.IsAny<SetTwoFactorEnabledModel>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel { Errors = TestErrors });

            SetupSignInMocks();

            var sut = CreateManageController();
            var result = await sut.EnableAuthenticator(new EnableAuthenticatorModel());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<EnableAuthenticatorModel>();
            sut.ModelState.ErrorCount.Should().Be(2);
        }

        [Test]
        public async Task
            POST_EnableAuthenticator_CheckRecoveryCodesStatusHasErrors_ReturnsViewResultWithErrorInModelState()
        {
            MockManageEndpoint.Setup(x => x.LoadSharedKeyAndQrCodeUriAsync(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatorKeyModel { StatusMessage = "OK" });
            MockManageEndpoint.Setup(x => x.VerifyTwoFactorTokenAsync(It.IsAny<VerifyTwoFactorTokenModel>()))
                .ReturnsAsync(new VerifyTwoFactorTokenModel { IsTokenValid = true, StatusMessage = "OK" });
            MockManageEndpoint.Setup(x => x.SetTwoFactorEnabledAsync(It.IsAny<SetTwoFactorEnabledModel>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel { StatusMessage = "OK" });
            MockManageEndpoint.Setup(x => x.CheckRecoveryCodesStatusAsync(It.IsAny<RecoveryCodeStatusModel>()))
                .ReturnsAsync(new RecoveryCodeStatusModel { Errors = TestErrors, StatusMessage = "Error" });

            SetupSignInMocks();

            var sut = CreateManageController();
            var result = await sut.EnableAuthenticator(new EnableAuthenticatorModel());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<EnableAuthenticatorModel>();
            sut.ModelState.ErrorCount.Should().Be(2);
        }

        [Test]
        public async Task POST_EnableAuthenticator_RecoveryCodesHaveUpdates_RedirectsToShowRecoveryCodes()
        {
            MockManageEndpoint.Setup(x => x.LoadSharedKeyAndQrCodeUriAsync(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatorKeyModel { StatusMessage = "OK" });
            MockManageEndpoint.Setup(x => x.VerifyTwoFactorTokenAsync(It.IsAny<VerifyTwoFactorTokenModel>()))
                .ReturnsAsync(new VerifyTwoFactorTokenModel { IsTokenValid = true, StatusMessage = "OK" });
            MockManageEndpoint.Setup(x => x.SetTwoFactorEnabledAsync(It.IsAny<SetTwoFactorEnabledModel>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel { StatusMessage = "OK" });
            MockManageEndpoint.Setup(x => x.CheckRecoveryCodesStatusAsync(It.IsAny<RecoveryCodeStatusModel>()))
                .ReturnsAsync(new RecoveryCodeStatusModel { IsUpdated = true, StatusMessage = "OK" });

            SetupSignInMocks();

            var sut = CreateManageController();
            var result = await sut.EnableAuthenticator(new EnableAuthenticatorModel());

            result.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = (RedirectToActionResult)result;
            redirectToActionResult.RouteValues.Count.Should().BeGreaterThan(0);
            redirectToActionResult.RouteValues["StatusMessage"].Should()
                .Be(ManageControllerStrings.AuthenticatorAppVerified);
            redirectToActionResult.ActionName.Should().Be(nameof(ManageController.ShowRecoveryCodes));
            sut.ModelState.ErrorCount.Should().Be(0);
        }

        [Test]
        public async Task POST_EnableAuthenticator_NoRecoveryCodeUpdate_RedirectsToTwoFactorAuthentication()
        {
            MockManageEndpoint.Setup(x => x.LoadSharedKeyAndQrCodeUriAsync(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatorKeyModel { StatusMessage = "OK" });
            MockManageEndpoint.Setup(x => x.VerifyTwoFactorTokenAsync(It.IsAny<VerifyTwoFactorTokenModel>()))
                .ReturnsAsync(new VerifyTwoFactorTokenModel { IsTokenValid = true, StatusMessage = "OK" });
            MockManageEndpoint.Setup(x => x.SetTwoFactorEnabledAsync(It.IsAny<SetTwoFactorEnabledModel>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel { StatusMessage = "OK" });
            MockManageEndpoint.Setup(x => x.CheckRecoveryCodesStatusAsync(It.IsAny<RecoveryCodeStatusModel>()))
                .ReturnsAsync(new RecoveryCodeStatusModel { IsUpdated = false, StatusMessage = "OK" });

            SetupSignInMocks();

            var sut = CreateManageController();
            var result = await sut.EnableAuthenticator(new EnableAuthenticatorModel());

            result.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = (RedirectToActionResult)result;
            redirectToActionResult.ActionName.Should().Be(nameof(ManageController.TwoFactorAuthentication));
            sut.ModelState.ErrorCount.Should().Be(0);
        }
    }
}