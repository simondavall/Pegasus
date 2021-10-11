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
            _mockSignInManager.Setup(x => x.SignOutAsync(It.IsAny<string>()));
            _mockApiHelper.Setup(x => x.RemoveTokenFromHeaders());

            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            var result = await sut.Logout("test-return-url");

            _mockSignInManager.Verify(x => x.SignOutAsync(It.IsAny<string>()), Times.Exactly(1));
            _mockApiHelper.Verify(x => x.RemoveTokenFromHeaders(), Times.Exactly(1));
            Assert.IsInstanceOf<LocalRedirectResult>(result);
            Assert.AreEqual("/Account/Login", ((LocalRedirectResult)result).Url);
        }
    }
}
