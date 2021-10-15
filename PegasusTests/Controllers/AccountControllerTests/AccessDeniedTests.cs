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
            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            var result = sut.AccessDenied();

            Assert.IsInstanceOf<ViewResult>(result);
        }
    }
}
