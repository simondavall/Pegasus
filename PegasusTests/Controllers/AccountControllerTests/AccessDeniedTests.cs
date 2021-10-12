using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Pegasus.Controllers;

namespace PegasusTests.Controllers.AccountControllerTests
{
    class AccessDeniedTests : AccountControllerTestsBase
    {
        [Test]
        public void GET_AccessDenied_ReturnsViewRequest()
        {
            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            var result = sut.AccessDenied();

            Assert.IsInstanceOf<ViewResult>(result);
        }
    }
}
