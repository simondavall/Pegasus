using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Library.Models.Manage;
using Pegasus.Library.Services.Resources;

namespace PegasusTests.Controllers.ManageControllerTests
{
    class SetPasswordTests : ManageControllerTestsBase
    {
        [Test]
        public async Task GET_SetPassword_HasErrors_ReturnsViewResultWithErrorMessage()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()))
                .ReturnsAsync(new HasPasswordModel {Errors = TestErrors, StatusMessage = "Error"});

            var sut = CreateManageController();
            var result = await sut.SetPassword();

            _mockApiHelper.Verify(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task GET_SetPassword_HasPassword_ReturnsRedirectsToChangePassword()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()))
                .ReturnsAsync(new HasPasswordModel {HasPassword = true, StatusMessage = "OK"});

            var sut = CreateManageController();
            var result = await sut.SetPassword();

            _mockApiHelper.Verify(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(nameof(ManageController.ChangePassword), ((RedirectToActionResult)result).ActionName);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task GET_SetPassword_HasNoPassword_ReturnsRedirectsToChangePassword()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()))
                .ReturnsAsync(new HasPasswordModel {HasPassword = false, StatusMessage = "OK"});

            var sut = CreateManageController();
            var result = await sut.SetPassword();

            _mockApiHelper.Verify(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_SetPassword_InvalidModel_ReturnsViewResult()
        {
            var sut = CreateManageController();
            sut.ModelState.AddModelError("Code", "Error");
            var result = await sut.SetPassword(new SetPasswordModel());

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<SetPasswordModel>(((ViewResult)result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_SetPassword_HasErrors_ReturnsViewResult()
        {
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<SetPasswordModel>(), It.IsAny<string>()))
                .ReturnsAsync(new SetPasswordModel {Errors = TestErrors, StatusMessage = "Error"});

            var sut = CreateManageController();
            var result = await sut.SetPassword(new SetPasswordModel());

            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<SetPasswordModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<SetPasswordModel>(((ViewResult)result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_SetPassword_NotSucceeded_ReturnsViewResultWithErrorMessage()
        {
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<SetPasswordModel>(), It.IsAny<string>()))
                .ReturnsAsync(new SetPasswordModel {Succeeded = false, StatusMessage = "OK"});

            var sut = CreateManageController();
            var result = await sut.SetPassword(new SetPasswordModel());

            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<SetPasswordModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<SetPasswordModel>(((ViewResult)result).Model);
            Assert.AreEqual(1, sut.ModelState.ErrorCount, "Error count failed.");
            Assert.AreEqual(Resources.ControllerStrings.ManageController.FailedToSetPassword,
                sut.ModelState.Values.First().Errors.First().ErrorMessage);
        }

        [Test]
        public async Task POST_SetPassword_Succeeded_ReturnsViewResult()
        {
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<SetPasswordModel>(), It.IsAny<string>()))
                .ReturnsAsync(new SetPasswordModel {Succeeded = true, StatusMessage = "OK"});

            SetupSignInMocks();

            var sut = CreateManageController();
            var result = await sut.SetPassword(new SetPasswordModel());

            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<SetPasswordModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<SetPasswordModel>(((ViewResult)result).Model);
            Assert.AreEqual(0, sut.ModelState.ErrorCount, "Error count failed.");
            var model = (SetPasswordModel)((ViewResult)result).Model;
            Assert.AreEqual(Resources.ControllerStrings.ManageController.PasswordSetSuccess, model.StatusMessage);

        }
    }
}
