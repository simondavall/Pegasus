using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Library.Models.Manage;

namespace PegasusTests.Controllers.ManageControllerTests
{
    internal class ResetAuthenticatorTests : ManageControllerTestsBase
    {
        [Test]
        public void GET_ResetAuthenticator_Called_ReturnsViewResult()
        {
            var sut = CreateManageController();
            var result = sut.ResetAuthenticator();

            result.Should().BeOfType<ViewResult>();
        }

        [Test]
        public async Task POST_ResetAuthenticator_HasErrors_ReturnsViewResultWithErrorMessage()
        {
            MockManageEndpoint.Setup(x => x.ResetAuthenticatorAsync(It.IsAny<ResetAuthenticatorModel>()))
                .ReturnsAsync(new ResetAuthenticatorModel { Errors = TestErrors, StatusMessage = "Error" });

            var sut = CreateManageController();
            var result = await sut.ResetAuthenticator(new ResetAuthenticatorModel());

            result.Should().BeOfType<ViewResult>();
            ((ViewResult)result).Model.Should().BeOfType<ResetAuthenticatorModel>();
            sut.ModelState.ErrorCount.Should().Be(2);
        }

        [Test]
        public async Task POST_ResetAuthenticator_Success_ReturnsRedirectToAction()
        {
            MockManageEndpoint.Setup(x => x.ResetAuthenticatorAsync(It.IsAny<ResetAuthenticatorModel>()))
                .ReturnsAsync(new ResetAuthenticatorModel { StatusMessage = "OK" });

            var sut = CreateManageController();
            var result = await sut.ResetAuthenticator(new ResetAuthenticatorModel());

            result.Should().BeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.Should().Be(nameof(ManageController.EnableAuthenticator));
            sut.ModelState.ErrorCount.Should().Be(0);
        }
    }
}