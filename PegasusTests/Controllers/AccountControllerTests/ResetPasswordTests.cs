using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Library.Models.Account;

namespace PegasusTests.Controllers.AccountControllerTests
{
    class ResetPasswordTests : AccountControllerTestsBase
    {
        [Test]
        public void GET_ResetPassword_NoCodeSupplied_ReturnsBadRequestObject()
        {
            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            var result = sut.ResetPassword();

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public void GET_ResetPassword_CodeSupplied_ReturnsViewRequest()
        {
            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            var result = sut.ResetPassword(null, "testCode");

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<ResetPasswordModel>(((ViewResult)result).Model);
        }

        [Test]
        public async Task POST_ResetPassword_InvalidModel_ReturnsViewRequestWithModelErrors()
        {
            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            sut.ModelState.AddModelError(string.Empty, string.Empty);
            var result = await sut.ResetPassword(new ResetPasswordModel());

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<ResetPasswordModel>(((ViewResult)result).Model);
        }

        [Test]
        public async Task POST_ResetPassword_ResetFailed_ReturnsViewRequestWithModelErrors()
        {
            MockAccountsEndpoint.Setup(x => x.ResetPassword(It.IsAny<ResetPasswordModel>()))
                .ReturnsAsync(new ResetPasswordModel{Succeeded = false, Errors = new List<IdentityError> {new IdentityError {Code="Code", Description = "Description"}}});

            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            var result = await sut.ResetPassword(new ResetPasswordModel());

            MockAccountsEndpoint.Verify(x => x.ResetPassword(It.IsAny<ResetPasswordModel>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<ResetPasswordModel>(((ViewResult)result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Failed error count.");
        }

        [Test]
        public async Task POST_ResetPassword_ResetSuccess_ReturnsViewRequestWithModelErrors()
        {
            MockAccountsEndpoint.Setup(x => x.ResetPassword(It.IsAny<ResetPasswordModel>()))
                .ReturnsAsync(new ResetPasswordModel{Succeeded = true});

            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            var result = await sut.ResetPassword(new ResetPasswordModel());

            MockAccountsEndpoint.Verify(x => x.ResetPassword(It.IsAny<ResetPasswordModel>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("ResetPasswordConfirmation", ((ViewResult)result).ViewName);
            Assert.Zero(sut.ModelState.ErrorCount, "Failed error count.");
        }
    }
}
