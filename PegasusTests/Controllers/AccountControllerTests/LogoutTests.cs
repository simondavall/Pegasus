using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;

namespace PegasusTests.Controllers.AccountControllerTests
{
    class LogoutTests : AccountControllerTestsBase
    {
        [Test]
        public async Task GET_Logout_ReturnsRedirectToLogin()
        {
            MockSignInManager.Setup(x => x.SignOutAsync(It.IsAny<string>()));
            MockApiHelper.Setup(x => x.RemoveTokenFromHeaders());

            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            var result = await sut.Logout("test-return-url");

            MockSignInManager.Verify(x => x.SignOutAsync(It.IsAny<string>()), Times.Exactly(1));
            MockApiHelper.Verify(x => x.RemoveTokenFromHeaders(), Times.Exactly(1));
            Assert.IsInstanceOf<LocalRedirectResult>(result);
            Assert.AreEqual("/Account/Login", ((LocalRedirectResult)result).Url);
        }
    }
}
