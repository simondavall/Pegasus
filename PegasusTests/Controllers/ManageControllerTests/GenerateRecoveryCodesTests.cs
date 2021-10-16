using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Library.Models.Manage;

namespace PegasusTests.Controllers.ManageControllerTests
{
    internal class GenerateRecoveryCodesTests : ManageControllerTestsBase
    {
        [Test]
        public async Task GET_GenerateRecoveryCodes_HasErrors_ReturnsViewResultWithErrorInModelState()
        {
            MockManageEndpoint.Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { Errors = TestErrors, StatusMessage = "Error" });

            var sut = CreateManageController();
            var result = await sut.GenerateRecoveryCodes();

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).ViewName.Should().Be(nameof(ManageController.GenerateRecoveryCodes));
            sut.ModelState.ErrorCount.Should().Be(2);
        }

        [Test]
        public async Task GET_GenerateRecoveryCodes_IsNotEnabled_ReturnsViewResultWithErrorInModelState()
        {
            MockManageEndpoint.Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = false, StatusMessage = "OK" });

            var sut = CreateManageController();
            var result = await sut.GenerateRecoveryCodes();

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).ViewName.Should().Be(nameof(ManageController.GenerateRecoveryCodes));
            sut.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public async Task GET_GenerateRecoveryCodes_IsEnabled_ReturnsViewResult()
        {
            MockManageEndpoint.Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = true, StatusMessage = "OK" });

            var sut = CreateManageController();
            var result = await sut.GenerateRecoveryCodes();

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).ViewName.Should().BeNull();
            sut.ModelState.ErrorCount.Should().Be(0);
        }

        [Test]
        public async Task POST_GenerateRecoveryCodes_HasErrors_ReturnsViewResultWithErrorInModelState()
        {
            MockManageEndpoint.Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { Errors = TestErrors, StatusMessage = "Error" });

            var sut = CreateManageController();
            var result = await sut.GenerateRecoveryCodesPost();

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).ViewName.Should().Be(nameof(ManageController.GenerateRecoveryCodes));
            sut.ModelState.ErrorCount.Should().Be(2);
        }

        [Test]
        public async Task POST_GenerateRecoveryCodes_IsNotEnabled_ReturnsViewResultWithErrorInModelState()
        {
            MockManageEndpoint.Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = false, StatusMessage = "OK" });

            var sut = CreateManageController();
            var result = await sut.GenerateRecoveryCodesPost();

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).ViewName.Should().Be(nameof(ManageController.GenerateRecoveryCodes));
            sut.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public async Task POST_GenerateRecoveryCodes_GenerateNewCodesHasErrors_ReturnsViewResultWithErrorInModelState()
        {
            MockManageEndpoint.Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = true, StatusMessage = "OK" });
            MockManageEndpoint.Setup(x => x.GenerateNewRecoveryCodesAsync(It.IsAny<string>()))
                .ReturnsAsync(new GenerateRecoveryCodesModel { Errors = TestErrors, StatusMessage = "Error" });

            var sut = CreateManageController();
            var result = await sut.GenerateRecoveryCodesPost();

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).ViewName.Should().Be(nameof(ManageController.GenerateRecoveryCodes));
            sut.ModelState.ErrorCount.Should().Be(2);
        }

        [Test]
        public async Task POST_GenerateRecoveryCodes_NewCodesGenerated_ReturnsRedirectToAction()
        {
            MockManageEndpoint.Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = true, StatusMessage = "OK" });
            MockManageEndpoint.Setup(x => x.GenerateNewRecoveryCodesAsync(It.IsAny<string>()))
                .ReturnsAsync(new GenerateRecoveryCodesModel { StatusMessage = "OK" });

            var sut = CreateManageController();
            var result = await sut.GenerateRecoveryCodesPost();

            result.Should().BeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.Should().Be(nameof(ManageController.ShowRecoveryCodes));
            sut.ModelState.ErrorCount.Should().Be(0);
        }
    }
}