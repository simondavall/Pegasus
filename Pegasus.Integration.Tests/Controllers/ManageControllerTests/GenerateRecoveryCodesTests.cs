using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Library.Models.Manage;

namespace Pegasus.Integration.Tests.Controllers.ManageControllerTests
{
    class GenerateRecoveryCodesTests : ManageControllerTestsBase
    {
        [Test]
        public async Task GET_GenerateRecoveryCodes_HasErrors_ReturnsViewResultWithErrorInModelState()
        {
            MockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel {Errors = TestErrors, StatusMessage = "Error"});
            
            var sut = CreateManageController();

            var result = await sut.GenerateRecoveryCodes();

            MockApiHelper.Verify(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(nameof(ManageController.GenerateRecoveryCodes), ((ViewResult)result).ViewName);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task GET_GenerateRecoveryCodes_IsNotEnabled_ReturnsViewResultWithErrorInModelState()
        {
            MockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = false, StatusMessage = "OK"});
            
            var sut = CreateManageController();

            var result = await sut.GenerateRecoveryCodes();

            MockApiHelper.Verify(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(nameof(ManageController.GenerateRecoveryCodes), ((ViewResult)result).ViewName);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task GET_GenerateRecoveryCodes_IsEnabled_ReturnsViewResult()
        {
            MockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = true, StatusMessage = "OK"});
            
            var sut = CreateManageController();

            var result = await sut.GenerateRecoveryCodes();

            MockApiHelper.Verify(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(null, ((ViewResult)result).ViewName);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_GenerateRecoveryCodes_HasErrors_ReturnsViewResultWithErrorInModelState()
        {
            MockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel {Errors = TestErrors, StatusMessage = "Error"});
            
            var sut = CreateManageController();

            var result = await sut.GenerateRecoveryCodesPost();

            MockApiHelper.Verify(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(nameof(ManageController.GenerateRecoveryCodes), ((ViewResult)result).ViewName);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_GenerateRecoveryCodes_IsNotEnabled_ReturnsViewResultWithErrorInModelState()
        {
            MockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = false, StatusMessage = "OK"});
            
            var sut = CreateManageController();

            var result = await sut.GenerateRecoveryCodesPost();

            MockApiHelper.Verify(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(nameof(ManageController.GenerateRecoveryCodes), ((ViewResult)result).ViewName);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_GenerateRecoveryCodes_GenerateNewCodesHasErrors_ReturnsViewResultWithErrorInModelState()
        {
            MockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = true, StatusMessage = "OK"});
            MockApiHelper.Setup(x => x.GetFromUri<GenerateRecoveryCodesModel>(It.IsAny<string>()))
                .ReturnsAsync(new GenerateRecoveryCodesModel { Errors = TestErrors, StatusMessage = "Error"});

            var sut = CreateManageController();

            var result = await sut.GenerateRecoveryCodesPost();

            MockApiHelper.Verify(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()), Times.Exactly(1));
            MockApiHelper.Verify(x => x.GetFromUri<GenerateRecoveryCodesModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(nameof(ManageController.GenerateRecoveryCodes), ((ViewResult)result).ViewName);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_GenerateRecoveryCodes_NewCodesGenerated_ReturnsRedirectToAction()
        {
            MockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = true, StatusMessage = "OK"});
            MockApiHelper.Setup(x => x.GetFromUri<GenerateRecoveryCodesModel>(It.IsAny<string>()))
                .ReturnsAsync(new GenerateRecoveryCodesModel { StatusMessage = "OK"});

            var sut = CreateManageController();

            var result = await sut.GenerateRecoveryCodesPost();

            MockApiHelper.Verify(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()), Times.Exactly(1));
            MockApiHelper.Verify(x => x.GetFromUri<GenerateRecoveryCodesModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(nameof(ManageController.ShowRecoveryCodes), ((RedirectToActionResult)result).ActionName);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed.");
        }
    }
}
