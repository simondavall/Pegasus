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
            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            var result = sut.Login();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task POST_Login_ModelInvalid_ReturnsViewResult()
        {
            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            sut.ModelState.AddModelError(string.Empty, string.Empty);
            var result = await sut.Login(new LoginViewModel(), string.Empty);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<LoginViewModel>(((ViewResult)result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed");
        }

        [Test]
        public async Task POST_Login_NotAuthenticated_ReturnsViewResultWithErrorMessage()
        {
            _mockAuthenticationEndpoint.Setup(x => x.Authenticate(It.IsAny<UserCredentials>()))
                .ReturnsAsync(new AuthenticatedUser { Authenticated = false });

            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            var result = await sut.Login(new LoginViewModel(), string.Empty);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<LoginViewModel>(((ViewResult)result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed");
        }

        [Test]
        public async Task POST_Login_SignInFailed_ReturnsViewResultWithErrorMessage()
        {
            _mockAuthenticationEndpoint.Setup(x => x.Authenticate(It.IsAny<UserCredentials>()))
                .ReturnsAsync(new AuthenticatedUser { Authenticated = false });
            _mockSignInManager.Setup(x => x.SignInOrTwoFactor(It.IsAny<AuthenticatedUser>()))
                .ReturnsAsync(new SignInResultModel { Success = false, RequiresTwoFactor = false});

            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            var result = await sut.Login(new LoginViewModel(), string.Empty);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<LoginViewModel>(((ViewResult)result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed");
        }

        // successes
        [Test]
        public async Task POST_Login_AuthenticateSuccessWithLocalUrl_ReturnsLocalRedirectToReturnUrl()
        {
            _mockAuthenticationEndpoint.Setup(x => x.Authenticate(It.IsAny<UserCredentials>()))
                .ReturnsAsync(new AuthenticatedUser { Authenticated = true });
            _mockSignInManager.Setup(x => x.SignInOrTwoFactor(It.IsAny<AuthenticatedUser>()))
                .ReturnsAsync(new SignInResultModel { Success = true });

            var returnUrl = "/";
            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            sut.Url = Url;
            var result = await sut.Login(new LoginViewModel(), returnUrl);

            Assert.IsInstanceOf<LocalRedirectResult>(result);
            Assert.AreEqual(returnUrl, ((LocalRedirectResult)result).Url);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed");
        }

        [Test]
        public async Task POST_Login_AuthenticateSuccessWithEmptyReturnUrl_ReturnsRedirectToAction()
        {
            _mockAuthenticationEndpoint.Setup(x => x.Authenticate(It.IsAny<UserCredentials>()))
                .ReturnsAsync(new AuthenticatedUser { Authenticated = true });
            _mockSignInManager.Setup(x => x.SignInOrTwoFactor(It.IsAny<AuthenticatedUser>()))
                .ReturnsAsync(new SignInResultModel { Success = true });

            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            sut.Url = Url;
            var result = await sut.Login(new LoginViewModel(), string.Empty);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(nameof(TaskListController.Index), ((RedirectToActionResult)result).ActionName);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed");
        }

        [Test]
        public async Task POST_Login_AuthenticateRequiresTwoFactor_ReturnsRedirectToAction()
        {
            _mockAuthenticationEndpoint.Setup(x => x.Authenticate(It.IsAny<UserCredentials>()))
                .ReturnsAsync(new AuthenticatedUser { Authenticated = true });
            _mockSignInManager.Setup(x => x.SignInOrTwoFactor(It.IsAny<AuthenticatedUser>()))
                .ReturnsAsync(new SignInResultModel { Success = false, RequiresTwoFactor = true});

            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            sut.Url = Url;
            var result = await sut.Login(new LoginViewModel(), string.Empty);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(nameof(AccountController.LoginWith2Fa), ((RedirectToActionResult)result).ActionName);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed");
        }
    }
}
