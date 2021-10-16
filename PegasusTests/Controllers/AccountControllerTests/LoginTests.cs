using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Library.JwtAuthentication.Models;
using Pegasus.Library.Models.Account;
using Pegasus.Models.Account;

namespace PegasusTests.Controllers.AccountControllerTests
{
    class LoginTests : AccountControllerTestsBase
    {
        [Test]
        public void GET_Login_ReturnsViewResult()
        {
            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            var result = sut.Login();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task POST_Login_ModelInvalid_ReturnsViewResult()
        {
            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            sut.ModelState.AddModelError(string.Empty, string.Empty);
            var result = await sut.Login(new LoginViewModel(), string.Empty);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<LoginViewModel>(((ViewResult)result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed");
        }

        [Test]
        public async Task POST_Login_NotAuthenticated_ReturnsViewResultWithErrorMessage()
        {
            MockAuthenticationEndpoint.Setup(x => x.Authenticate(It.IsAny<UserCredentials>()))
                .ReturnsAsync(new AuthenticatedUser { Authenticated = false });

            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            var result = await sut.Login(new LoginViewModel(), string.Empty);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<LoginViewModel>(((ViewResult)result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed");
        }

        [Test]
        public async Task POST_Login_SignInFailed_ReturnsViewResultWithErrorMessage()
        {
            MockAuthenticationEndpoint.Setup(x => x.Authenticate(It.IsAny<UserCredentials>()))
                .ReturnsAsync(new AuthenticatedUser { Authenticated = false });
            MockSignInManager.Setup(x => x.SignInOrTwoFactor(It.IsAny<AuthenticatedUser>()))
                .ReturnsAsync(new SignInResultModel { Success = false, RequiresTwoFactor = false});

            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            var result = await sut.Login(new LoginViewModel(), string.Empty);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<LoginViewModel>(((ViewResult)result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed");
        }

        // successes
        [Test]
        public async Task POST_Login_AuthenticateSuccessWithLocalUrl_ReturnsLocalRedirectToReturnUrl()
        {
            MockAuthenticationEndpoint.Setup(x => x.Authenticate(It.IsAny<UserCredentials>()))
                .ReturnsAsync(new AuthenticatedUser { Authenticated = true });
            MockSignInManager.Setup(x => x.SignInOrTwoFactor(It.IsAny<AuthenticatedUser>()))
                .ReturnsAsync(new SignInResultModel { Success = true });

            var returnUrl = "/";
            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            sut.Url = GetUrl();
            var result = await sut.Login(new LoginViewModel(), returnUrl);

            Assert.IsInstanceOf<LocalRedirectResult>(result);
            Assert.AreEqual(returnUrl, ((LocalRedirectResult)result).Url);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed");
        }

        [Test]
        public async Task POST_Login_AuthenticateSuccessWithEmptyReturnUrl_ReturnsRedirectToAction()
        {
            MockAuthenticationEndpoint.Setup(x => x.Authenticate(It.IsAny<UserCredentials>()))
                .ReturnsAsync(new AuthenticatedUser { Authenticated = true });
            MockSignInManager.Setup(x => x.SignInOrTwoFactor(It.IsAny<AuthenticatedUser>()))
                .ReturnsAsync(new SignInResultModel { Success = true });

            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            sut.Url = GetUrl();
            var result = await sut.Login(new LoginViewModel(), string.Empty);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(nameof(TaskListController.Index), ((RedirectToActionResult)result).ActionName);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed");
        }

        [Test]
        public async Task POST_Login_AuthenticateRequiresTwoFactor_ReturnsRedirectToAction()
        {
            MockAuthenticationEndpoint.Setup(x => x.Authenticate(It.IsAny<UserCredentials>()))
                .ReturnsAsync(new AuthenticatedUser { Authenticated = true });
            MockSignInManager.Setup(x => x.SignInOrTwoFactor(It.IsAny<AuthenticatedUser>()))
                .ReturnsAsync(new SignInResultModel { Success = false, RequiresTwoFactor = true});

            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            var result = await sut.Login(new LoginViewModel(), string.Empty);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(nameof(AccountController.LoginWith2Fa), ((RedirectToActionResult)result).ActionName);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed");
        }

        [Test]
        public async Task POST_Login_SignInFailsAndNo2FaRequestRequired_ReturnsViewRequestWithErrors()
        {
            MockAuthenticationEndpoint.Setup(x => x.Authenticate(It.IsAny<UserCredentials>()))
                .ReturnsAsync(new AuthenticatedUser { Authenticated = true });
            MockSignInManager.Setup(x => x.SignInOrTwoFactor(It.IsAny<AuthenticatedUser>()))
                .ReturnsAsync(new SignInResultModel { Success = false, RequiresTwoFactor = false});

            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            var result = await sut.Login(new LoginViewModel(), string.Empty);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<LoginViewModel>(((ViewResult)result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed");
        }

        private static IUrlHelper GetUrl()
        {
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x => x.IsLocalUrl(It.IsAny<string>())).Returns(true);
            mockUrlHelper.Setup(x => x.IsLocalUrl(string.Empty)).Returns(false);
            mockUrlHelper.Setup(x => x.IsLocalUrl(null)).Returns(false);
            return mockUrlHelper.Object;
        }
    }
}
