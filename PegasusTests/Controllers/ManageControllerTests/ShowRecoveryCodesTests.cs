using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Library.Models.Manage;

namespace PegasusTests.Controllers.ManageControllerTests
{
    internal class ShowRecoveryCodesTests : ManageControllerTestsBase
    {
        [Test]
        public void GET_ShowRecoveryCodes_NoRecoveryCodes_ReturnsRedirectToAction()
        {
            var sut = CreateManageController();
            var result = sut.ShowRecoveryCodes(new ShowRecoveryCodesModel());

            result.Should().BeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.Should().Be(nameof(ManageController.TwoFactorAuthentication));
        }

        [Test]
        public void GET_ShowRecoveryCodes_WithRecoveryCodes_ReturnsViewResult()
        {
            var model = new ShowRecoveryCodesModel { RecoveryCodes = new[] { "test code" } };

            var sut = CreateManageController();
            var result = sut.ShowRecoveryCodes(model);

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<ShowRecoveryCodesModel>();
        }
    }
}