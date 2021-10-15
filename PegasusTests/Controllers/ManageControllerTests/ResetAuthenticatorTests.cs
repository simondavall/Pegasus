using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Library.Models.Manage;

namespace PegasusTests.Controllers.ManageControllerTests
{
    class ResetAuthenticatorTests : ManageControllerTestsBase
    {
        [Test]
        public void GET_ResetAuthenticator_Called_ReturnsViewResult()
        {
            var sut = CreateManageController();
            var result = sut.ResetAuthenticator();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task POST_ResetAuthenticator_HasErrors_ReturnsViewResultWithErrorMessage()
        {
            MockApiHelper.Setup(x => x.PostAsync(It.IsAny<ResetAuthenticatorModel>(), It.IsAny<string>()))
                .ReturnsAsync(new ResetAuthenticatorModel {Errors = TestErrors, StatusMessage = "Error"});

            var sut = CreateManageController();
            var result = await sut.ResetAuthenticator(new ResetAuthenticatorModel());

            MockApiHelper.Verify(x => x.PostAsync(It.IsAny<ResetAuthenticatorModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<ResetAuthenticatorModel>(((ViewResult) result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_ResetAuthenticator_Success_ReturnsRedirectToAction()
        {
            MockApiHelper.Setup(x => x.PostAsync(It.IsAny<ResetAuthenticatorModel>(), It.IsAny<string>()))
                .ReturnsAsync(new ResetAuthenticatorModel { StatusMessage = "OK"});

            var sut = CreateManageController();
            var result = await sut.ResetAuthenticator(new ResetAuthenticatorModel());

            MockApiHelper.Verify(x => x.PostAsync(It.IsAny<ResetAuthenticatorModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(nameof(ManageController.EnableAuthenticator),((RedirectToActionResult)result).ActionName);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed.");
        }
    }
}
