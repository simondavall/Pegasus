using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Library.Models.Manage;

namespace PegasusTests.Controllers.ManageControllerTests
{
    class ShowRecoveryCodesTests : ManageControllerTestsBase
    {
        [Test]
        public void GET_ShowRecoveryCodes_NoRecoveryCodes_ReturnsRedirectToAction()
        {
            var sut = CreateManageController();
            var result = sut.ShowRecoveryCodes(new ShowRecoveryCodesModel());

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(nameof(ManageController.TwoFactorAuthentication), ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public void GET_ShowRecoveryCodes_WithRecoveryCodes_ReturnsViewResult()
        {
            var model = new ShowRecoveryCodesModel { RecoveryCodes = new[] { "test code" } };

            var sut = CreateManageController();
            var result = sut.ShowRecoveryCodes(model);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<ShowRecoveryCodesModel>(((ViewResult)result).Model);
        }
    }
}
