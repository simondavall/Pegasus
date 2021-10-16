using System.Linq;
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
    internal class SetPasswordTests : ManageControllerTestsBase
    {
        [Test]
        public async Task GET_SetPassword_HasErrors_ReturnsViewResultWithErrorMessage()
        {
            MockManageEndpoint.Setup(x => x.HasPasswordAsync(It.IsAny<string>()))
                .ReturnsAsync(new HasPasswordModel { Errors = TestErrors, StatusMessage = "Error" });

            var sut = CreateManageController();
            var result = await sut.SetPassword();

            result.Should().BeOfType<ViewResult>();
            sut.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public async Task GET_SetPassword_HasPassword_ReturnsRedirectsToChangePassword()
        {
            MockManageEndpoint.Setup(x => x.HasPasswordAsync(It.IsAny<string>()))
                .ReturnsAsync(new HasPasswordModel { HasPassword = true, StatusMessage = "OK" });

            var sut = CreateManageController();
            var result = await sut.SetPassword();

            result.Should().BeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.Should().Be(nameof(ManageController.ChangePassword));
            sut.ModelState.ErrorCount.Should().Be(0);
        }

        [Test]
        public async Task GET_SetPassword_HasNoPassword_ReturnsRedirectsToChangePassword()
        {
            MockManageEndpoint.Setup(x => x.HasPasswordAsync(It.IsAny<string>()))
                .ReturnsAsync(new HasPasswordModel { HasPassword = false, StatusMessage = "OK" });

            var sut = CreateManageController();
            var result = await sut.SetPassword();

            result.Should().BeOfType<ViewResult>();
            sut.ModelState.ErrorCount.Should().Be(0);
        }

        [Test]
        public async Task POST_SetPassword_InvalidModel_ReturnsViewResult()
        {
            var sut = CreateManageController();
            sut.ModelState.AddModelError("Code", "Error");
            var result = await sut.SetPassword(new SetPasswordModel());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<SetPasswordModel>();
            sut.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public async Task POST_SetPassword_HasErrors_ReturnsViewResult()
        {
            MockManageEndpoint.Setup(x => x.AddPasswordAsync(It.IsAny<SetPasswordModel>()))
                .ReturnsAsync(new SetPasswordModel { Errors = TestErrors, StatusMessage = "Error" });

            var sut = CreateManageController();
            var result = await sut.SetPassword(new SetPasswordModel());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<SetPasswordModel>();
            sut.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public async Task POST_SetPassword_NotSucceeded_ReturnsViewResultWithErrorMessage()
        {
            MockManageEndpoint.Setup(x => x.AddPasswordAsync(It.IsAny<SetPasswordModel>()))
                .ReturnsAsync(new SetPasswordModel { Succeeded = false, StatusMessage = "OK" });

            var sut = CreateManageController();
            var result = await sut.SetPassword(new SetPasswordModel());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<SetPasswordModel>();
            sut.ModelState.ErrorCount.Should().Be(1);
            var modelError = sut.ModelState.Values.First().Errors.First();
            modelError.ErrorMessage.Should().Be(ManageControllerStrings.FailedToSetPassword);
        }

        [Test]
        public async Task POST_SetPassword_Succeeded_ReturnsViewResult()
        {
            MockManageEndpoint.Setup(x => x.AddPasswordAsync(It.IsAny<SetPasswordModel>()))
                .ReturnsAsync(new SetPasswordModel { Succeeded = true, StatusMessage = "OK" });

            SetupSignInMocks();

            var sut = CreateManageController();
            var result = await sut.SetPassword(new SetPasswordModel());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<SetPasswordModel>();
            sut.ModelState.ErrorCount.Should().Be(0);
            var setPasswordModel = (SetPasswordModel)((ViewResult)result).Model;
            setPasswordModel.StatusMessage.Should().Be(ManageControllerStrings.PasswordSetSuccess);
        }
    }
}