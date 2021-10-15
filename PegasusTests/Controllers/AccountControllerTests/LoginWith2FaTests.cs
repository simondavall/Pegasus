using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Library.Models.Account;
using Pegasus.Models.Account;
using AccountControllerStrings = Pegasus.Library.Services.Resources.Resources.ControllerStrings.AccountController;

namespace PegasusTests.Controllers.AccountControllerTests
{
    class LoginWith2FaTests : AccountControllerTestsBase
    {
        [Test]
        public async Task GET_LoginWith2Fa_UserIsNull_ReturnsViewResult()
        {
            MockSignInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync())
                .ReturnsAsync((string)null);

            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            var result = await sut.LoginWith2Fa();

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(1, sut.ModelState.ErrorCount, "Error count failed");
            Assert.AreEqual(AccountControllerStrings.CannotFind2FaUser,
                sut.ModelState.Values.First().Errors.First().ErrorMessage);
        }

        [Test]
        public async Task GET_LoginWith2Fa_UserNotNull_ReturnsViewResult()
        {
            MockSignInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync())
                .ReturnsAsync("test-user");

            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            var returnUrl = "test-return-url";
            var result = await sut.LoginWith2Fa(returnUrl);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<LoginWith2FaViewModel>(((ViewResult)result).Model);
            Assert.AreEqual(returnUrl, ((LoginWith2FaViewModel)((ViewResult)result).Model).ReturnUrl);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed");
        }

        [Test]
        public async Task POST_LoginWith2Fa_InvalidModel_ReturnsViewResultWithErrorMessage()
        {
            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            sut.ModelState.AddModelError(string.Empty, string.Empty);
            var result = await sut.LoginWith2Fa(new LoginWith2FaViewModel());

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(1, sut.ModelState.ErrorCount, "Error count failed");
        }

        [Test]
        public async Task POST_LoginWith2Fa_UserIsNull_ReturnsViewResult()
        {
            MockSignInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync())
                .ReturnsAsync((string)null);

            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            var result = await sut.LoginWith2Fa(new LoginWith2FaViewModel());

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual(1, sut.ModelState.ErrorCount, "Error count failed");
            Assert.AreEqual(AccountControllerStrings.CannotFind2FaUser,
                sut.ModelState.Values.First().Errors.First().ErrorMessage);
        }

        [Test]
        public async Task POST_LoginWith2Fa_TokenVerificationFailed_ReturnsViewResultWithErrorMessage()
        {
            MockSignInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync())
                .ReturnsAsync("test-user");
            MockAccountsEndpoint.Setup(x => x.VerifyTwoFactorTokenAsync(It.IsAny<VerifyTwoFactorModel>()))
                .ReturnsAsync(new VerifyTwoFactorModel {Verified = false});

            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            var result = await sut.LoginWith2Fa(new LoginWith2FaViewModel());

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<LoginWith2FaViewModel>(((ViewResult)result).Model);
            Assert.AreEqual(1, sut.ModelState.ErrorCount, "Error count failed");
            Assert.AreEqual(AccountControllerStrings.InvalidAuthenticationCode,
                sut.ModelState.Values.First().Errors.First().ErrorMessage);
        }

        [Test]
        public async Task POST_LoginWith2Fa_TokenVerificationSuccess_ReturnsRedirectToReturnUrl()
        {
            MockSignInManager.Setup(x => x.GetTwoFactorAuthenticationUserAsync())
                .ReturnsAsync("test-user");
            MockSignInManager.Setup(x => x.DoTwoFactorSignInAsync(It.IsAny<string>(), It.IsAny<bool>()));
            MockAccountsEndpoint.Setup(x => x.VerifyTwoFactorTokenAsync(It.IsAny<VerifyTwoFactorModel>()))
                .ReturnsAsync(new VerifyTwoFactorModel {Verified = true});

            var sut = new AccountController(MockLogger.Object, MockApiHelper.Object, MockSignInManager.Object, MockAccountsEndpoint.Object, MockAuthenticationEndpoint.Object);
            var result = await sut.LoginWith2Fa(new LoginWith2FaViewModel {ReturnUrl = "/"});

            Assert.IsInstanceOf<LocalRedirectResult>(result);
            Assert.AreEqual("/", ((LocalRedirectResult)result).Url);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed");
        }
    }
}
