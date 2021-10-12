using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Models.Account;
using AccountControllerStrings = Pegasus.Library.Services.Resources.Resources.ControllerStrings.AccountController;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace PegasusTests.Controllers.AccountControllerTests
{
    class LoginWithRecoveryCodeTests : AccountControllerTestsBase
    {
        [Test]
        public async Task GET_LoginWithRecoveryCode_UserIsNull_ReturnsViewResult()
        {
            _mockSignInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync())
                .ReturnsAsync((string)null);

            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            var result = await sut.LoginWithRecoveryCode();

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(1, sut.ModelState.ErrorCount, "Error count failed");
            Assert.AreEqual(AccountControllerStrings.CannotFind2FaUser,
                sut.ModelState.Values.First().Errors.First().ErrorMessage);
        }

        [Test]
        public async Task GET_LoginWithRecoveryCode_UserNotNull_ReturnsViewResult()
        {
            _mockSignInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync())
                .ReturnsAsync("test-user");

            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            var returnUrl = "test-return-url";
            var result = await sut.LoginWithRecoveryCode(returnUrl);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<LoginWithRecoveryCodeModel>(((ViewResult)result).Model);
            Assert.AreEqual(returnUrl, ((LoginWithRecoveryCodeModel)((ViewResult)result).Model).ReturnUrl);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed");
        }

        [Test]
        public async Task POST_LoginWithRecoveryCode_InvalidModel_ReturnsViewResultWithErrorMessage()
        {
            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            sut.ModelState.AddModelError(string.Empty, string.Empty);
            var result = await sut.LoginWithRecoveryCode(new LoginWithRecoveryCodeModel());

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(1, sut.ModelState.ErrorCount, "Error count failed");
        }

        [Test]
        public async Task POST_LoginWithRecoveryCode_UserIsNull_ReturnsViewResult()
        {
            _mockSignInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync())
                .ReturnsAsync((string)null);

            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            var result = await sut.LoginWithRecoveryCode(new LoginWithRecoveryCodeModel());

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(1, sut.ModelState.ErrorCount, "Error count failed");
            Assert.AreEqual(AccountControllerStrings.CannotFind2FaUser, sut.ModelState.Values.First().Errors.First().ErrorMessage);
        }

        [Test]
        public async Task POST_LoginWithRecoveryCode_LoginFailed_ReturnsViewResult()
        {
            _mockSignInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync())
                .ReturnsAsync("test-user");
            _mockSignInManager.Setup(x => x.TwoFactorRecoveryCodeSignInAsync(It.IsAny<string>()))
                .ReturnsAsync(SignInResult.Failed);

            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            var result = await sut.LoginWithRecoveryCode(new LoginWithRecoveryCodeModel());

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(1, sut.ModelState.ErrorCount, "Error count failed");
            Assert.AreEqual(AccountControllerStrings.InvalidRecoveryCode, sut.ModelState.Values.First().Errors.First().ErrorMessage);
        }

        [Test]
        public async Task POST_LoginWithRecoveryCode_LoginSuccess_ReturnsViewResult()
        {
            _mockSignInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync())
                .ReturnsAsync("test-user");
            _mockSignInManager.Setup(x => x.TwoFactorRecoveryCodeSignInAsync(It.IsAny<string>()))
                .ReturnsAsync(SignInResult.Success);

            var sut = new AccountController(_mockLogger.Object, _mockApiHelper.Object, _mockSignInManager.Object, _mockAccountsEndpoint.Object, _mockAuthenticationEndpoint.Object);
            var result = await sut.LoginWithRecoveryCode(new LoginWithRecoveryCodeModel { RecoveryCode = "test-recovery-code", ReturnUrl = "/"});

            Assert.IsInstanceOf<LocalRedirectResult>(result);
            Assert.AreEqual("/", ((LocalRedirectResult)result).Url);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed");
        }
    }
}
