using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Library.Models.Manage;
using Pegasus.Library.Services.Resources;

namespace Pegasus.Integration.Tests.Controllers.ManageControllerTests
{
    class Disable2FaTests : ManageControllerTestsBase
    {
        [Test]
        public async Task GET_Disable2Fa_IsEnabled_ReturnsViewResult()
        {
            MockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = true});
            
            var sut = CreateManageController();
            var result = await sut.Disable2Fa();

            MockApiHelper.Verify(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.Zero(sut.ModelState.ErrorCount,  "Failed error count");
        }

        [Test]
        public async Task GET_Disable2Fa_IsNotEnabled_ReturnsViewResultWithMessage()
        {
            MockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = false});
            
            var sut = CreateManageController();
            var result = await sut.Disable2Fa();

            MockApiHelper.Verify(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(Resources.ControllerStrings.ManageController.TwoFactorNotEnabled, ((Disable2FaModel)((ViewResult)result).Model).StatusMessage);
        }

        [Test]
        public async Task POST_Disable2Fa_IsNotEnabled_ReturnsViewResultWithMessage()
        {
            MockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = false});
            MockApiHelper.Setup(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel {Succeeded = false, Errors = TestErrors, StatusMessage = "Error"});
            
            var sut = CreateManageController();
            var result = await sut.Disable2Fa(new Disable2FaModel());

            MockApiHelper.Verify(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(Resources.ControllerStrings.ManageController.TwoFactorNotEnabled, ((Disable2FaModel)((ViewResult)result).Model).StatusMessage);
        }

        [Test]
        public async Task POST_Disable2Fa_NotSucceeded_ReturnsViewResultWithMessage()
        {
            MockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = true});
            MockApiHelper.Setup(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel {Succeeded = false, Errors = TestErrors, StatusMessage = "Error"});
            
            var sut = CreateManageController();
            var result = await sut.Disable2Fa(new Disable2FaModel());

            MockApiHelper.Verify(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<Disable2FaModel>(((ViewResult) result).Model);
            var taskResult = (Disable2FaModel)((ViewResult)result).Model;
            Assert.AreEqual(Resources.ControllerStrings.ManageController.FailedToDisable2Fa, taskResult.StatusMessage);
        }

        [Test]
        public async Task POST_Disable2Fa_NSucceeded_ReturnsRedirectToAction()
        {
            MockApiHelper.Setup(x => x.GetFromUri<GetTwoFactorEnabledModel>(It.IsAny<string>()))
                .ReturnsAsync(new GetTwoFactorEnabledModel { IsEnabled = true});
            MockApiHelper.Setup(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()))
                .ReturnsAsync(new SetTwoFactorEnabledModel {Succeeded = true, StatusMessage = "OK"});
            
            var sut = CreateManageController();
            var result = await sut.Disable2Fa(new Disable2FaModel());

            MockApiHelper.Verify(x => x.PostAsync(It.IsAny<SetTwoFactorEnabledModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.Zero(sut.ModelState.ErrorCount,"Failed error count");
        }
    }
}
