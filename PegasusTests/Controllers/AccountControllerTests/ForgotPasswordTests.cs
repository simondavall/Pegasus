using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Library.Models.Account;

namespace PegasusTests.Controllers.AccountControllerTests
{
    class ForgotPasswordTests : AccountControllerTestsBase
    {
        [Test]
        public void GET_ForgotPassword_ReturnsViewResult()
        {
            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            var result = sut.ForgotPassword();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task POST_ForgotPassword_InvalidModel_ReturnsViewResult()
        {
            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            sut.ModelState.AddModelError(string.Empty, string.Empty);
            var result = await sut.ForgotPassword(new ForgotPasswordModel());

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task POST_ForgotPassword_ReturnsViewResultForgotConfirmation()
        {
            // mock the call to Url.ResetPasswordBaseUrl(Request.Scheme)
            var mockUrl = new Mock<IUrlHelper>();
            mockUrl.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns("new-base-url");

            var controllerContext = new ControllerContext() {
                HttpContext = new DefaultHttpContext { Request = { Scheme = "fake_scheme" } },
            };
            // end mock

            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object,
                _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object)
            {
                ControllerContext = controllerContext
            };
            sut.Url = mockUrl.Object;
            var result = await sut.ForgotPassword(new ForgotPasswordModel());

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("ForgotPasswordConfirmation", ((ViewResult)result).ViewName);
        }
    }
}
