using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Library.Models.Manage;
using ManageControllerStrings = Pegasus.Library.Services.Resources.Resources.ControllerStrings.ManageController;

namespace PegasusTests.Controllers.ManageControllerTests
{
    internal class Disable2FaTests : ManageControllerTestsBase
    {
        [Test]
        public async Task GET_Disable2Fa_IsEnabled_ReturnsViewResult()
        {
            MockManageEndpoint.Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = true });

            var sut = CreateManageController();
            var result = await sut.Disable2Fa();

            result.Should().BeOfType<ViewResult>();
            sut.ModelState.ErrorCount.Should().Be(0);
        }

        [Test]
        public async Task GET_Disable2Fa_IsNotEnabled_ReturnsViewResultWithMessage()
        {
            MockManageEndpoint.Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = false });

            var sut = CreateManageController();
            var result = await sut.Disable2Fa();

            result.Should().BeOfType<ViewResult>();
        }

        [Test]
        public async Task POST_Disable2Fa_IsNotEnabled_ReturnsViewResultWithMessage()
        {
            MockManageEndpoint.Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = false });
            MockManageEndpoint.Setup(x => x.SetTwoFactorEnabledAsync(It.IsAny<SetTwoFactorEnabledModel>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel
                    { Succeeded = false, Errors = TestErrors, StatusMessage = "Error" });

            var sut = CreateManageController();
            var result = await sut.Disable2Fa(new Disable2FaModel());

            result.Should().BeOfType<ViewResult>();
            var disable2FaModel = (Disable2FaModel)((ViewResult)result).Model;
            disable2FaModel.StatusMessage.Should().Be(ManageControllerStrings.TwoFactorNotEnabled);
        }

        [Test]
        public async Task POST_Disable2Fa_NotSucceeded_ReturnsViewResultWithMessage()
        {
            MockManageEndpoint.Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = true });
            MockManageEndpoint.Setup(x => x.SetTwoFactorEnabledAsync(It.IsAny<SetTwoFactorEnabledModel>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel
                    { Succeeded = false, Errors = TestErrors, StatusMessage = "Error" });

            var sut = CreateManageController();
            var result = await sut.Disable2Fa(new Disable2FaModel());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<Disable2FaModel>();
            var disable2FaModel = (Disable2FaModel)((ViewResult)result).Model;
            disable2FaModel.StatusMessage.Should().Be(ManageControllerStrings.FailedToDisable2Fa);
            sut.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public async Task POST_Disable2Fa_NSucceeded_ReturnsRedirectToAction()
        {
            MockManageEndpoint.Setup(x => x.GetTwoFactorEnabledAsync(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = true });
            MockManageEndpoint.Setup(x => x.SetTwoFactorEnabledAsync(It.IsAny<SetTwoFactorEnabledModel>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel { Succeeded = true, StatusMessage = "OK" });

            var sut = CreateManageController();
            var result = await sut.Disable2Fa(new Disable2FaModel());

            result.Should().BeOfType<RedirectToActionResult>();
            sut.ModelState.ErrorCount.Should().Be(0);
        }
    }
}