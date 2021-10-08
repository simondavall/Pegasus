using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Pegasus.Library.Api;
using Pegasus.Library.JwtAuthentication;
using Pegasus.Library.JwtAuthentication.Models;
using Pegasus.Library.Models.Manage;
using Pegasus.Library.Services.Http;
using Pegasus.Services;

namespace PegasusTests.Controllers
{
    class ManageControllerTests
    {
        private ControllerContext _controllerContext;

        private Mock<IHttpContextWrapper> _mockHttpContextWrapper;
        private Mock<IAccountsEndpoint> _mockAccountsEndpoint;
        private Mock<IApiHelper> _mockApiHelper;
        private Mock<IJwtTokenAccessor> _mockTokenAccessor;
        private Mock<IAuthenticationEndpoint> _mockAuthenticationEndpoint;
        private Mock<ILogger<Pegasus.Controllers.ManageController>> _logger;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _mockHttpContextWrapper = new Mock<IHttpContextWrapper>();
            _mockTokenAccessor = new Mock<IJwtTokenAccessor>();
            _mockAuthenticationEndpoint = new Mock<IAuthenticationEndpoint>();
            _logger = new Mock<ILogger<Pegasus.Controllers.ManageController>>();
        }

        [SetUp]
        public void TestSetup()
        {
            _mockApiHelper = new Mock<IApiHelper>();
            _mockAccountsEndpoint = new Mock<IAccountsEndpoint>();
            SetupAuthenticationMock();
            SetupContextUser();
        }

        [Test]
        public async Task TwoFactorAuthentication_CallsApi_ReturnsViewResult()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>())).ReturnsAsync(new TwoFactorAuthenticationModel());
            var manageEndpoint = new ManageEndpoint(_mockApiHelper.Object);

            var signInManager = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

            var sut = new Pegasus.Controllers.ManageController(manageEndpoint, signInManager, _logger.Object)
            {
                ControllerContext = _controllerContext
            };

            var result = await sut.TwoFactorAuthentication(new TwoFactorAuthenticationModel());

            _mockHttpContextWrapper.Verify(x => x.SignOutAsync(It.IsAny<string>()), Times.Exactly(1));
            _mockApiHelper.Verify(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task ChangePassword_HasErrors_ReturnsViewResult()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()))
                .ReturnsAsync(new HasPasswordModel {Errors = new List<IdentityError> {new IdentityError {Description = "Error Message"}}, StatusMessage = "Error"});
            var manageEndpoint = new ManageEndpoint(_mockApiHelper.Object);

            var signInManager = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

            var sut = new Pegasus.Controllers.ManageController(manageEndpoint, signInManager, _logger.Object)
            {
                ControllerContext = _controllerContext
            };

            var result = await sut.ChangePassword();

            _mockApiHelper.Verify(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task ChangePassword_HasNoPassword_RedirectsToSetPassword()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()))
                .ReturnsAsync(new HasPasswordModel {StatusMessage = "OK", HasPassword = false});
            var manageEndpoint = new ManageEndpoint(_mockApiHelper.Object);

            var signInManager = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

            var sut = new Pegasus.Controllers.ManageController(manageEndpoint, signInManager, _logger.Object)
            {
                ControllerContext = _controllerContext
            };

            var result = await sut.ChangePassword();

            _mockApiHelper.Verify(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(nameof(sut.SetPassword),((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task ChangePassword_HasPassword_ReturnsViewResult()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()))
                .ReturnsAsync(new HasPasswordModel {StatusMessage = "OK", HasPassword = true});
            var manageEndpoint = new ManageEndpoint(_mockApiHelper.Object);

            var signInManager = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object, _mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

            var sut = new Pegasus.Controllers.ManageController(manageEndpoint, signInManager, _logger.Object)
            {
                ControllerContext = _controllerContext
            };

            var result = await sut.ChangePassword();

            _mockApiHelper.Verify(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task ChangePassword_InvalidModelState_ReturnsViewResult()
        {
            var manageEndpoint = new ManageEndpoint(_mockApiHelper.Object);
            var signInManager = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object, _mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

            var sut = new Pegasus.Controllers.ManageController(manageEndpoint, signInManager, _logger.Object)
            {
                ControllerContext = _controllerContext
            };
            sut.ModelState.AddModelError("Error", "An error occurred.");
            var result = await sut.ChangePassword(new ChangePasswordModel());

            _mockApiHelper.Verify(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()), Times.Exactly(0));
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task ChangePassword_PostHasErrors_ReturnsViewResultWithErrorInModelState()
        {
            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<ChangePasswordModel>(), It.IsAny<string>()))
                .ReturnsAsync(new ChangePasswordModel {Succeeded = false, Errors = new List<IdentityError> {new IdentityError {Description = "Error Message"}}, StatusMessage = "Error"});
            var manageEndpoint = new ManageEndpoint(_mockApiHelper.Object);

            var signInManager = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

            var sut = new Pegasus.Controllers.ManageController(manageEndpoint, signInManager, _logger.Object)
            {
                ControllerContext = _controllerContext
            };

            var result = await sut.ChangePassword(new ChangePasswordModel());

            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<ChangePasswordModel>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<ChangePasswordModel>(((ViewResult) result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount);
        }

        [Test]
        public async Task ChangePassword_NoErrors_ReturnsViewResultWithErrorInModelState()
        {
            var authResult = GetAuthenticateResult();
            _mockHttpContextWrapper.Setup(x => x.AuthenticateAsync(It.IsAny<string>())).ReturnsAsync(authResult);

            _mockAuthenticationEndpoint.Setup(x => x.Authenticate2Fa(It.IsAny<string>())).ReturnsAsync(new AuthenticatedUser {AccessToken = "access-token", Authenticated = true});

            _mockTokenAccessor.Setup(x => x.GetAccessTokenWithClaimsPrincipal(It.IsAny<AuthenticatedUser>())).Returns(new TokenWithClaimsPrincipal());

            _mockApiHelper.Setup(x => x.PostAsync(It.IsAny<ChangePasswordModel>(), It.IsAny<string>()))
                .ReturnsAsync(new ChangePasswordModel {Succeeded = true, StatusMessage = "OK"});
            var manageEndpoint = new ManageEndpoint(_mockApiHelper.Object);

            var signInManager = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object, _mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

            var sut = new Pegasus.Controllers.ManageController(manageEndpoint, signInManager, _logger.Object)
            {
                ControllerContext = _controllerContext
            };

            var result = await sut.ChangePassword(new ChangePasswordModel());

            _mockApiHelper.Verify(x => x.PostAsync(It.IsAny<ChangePasswordModel>(), It.IsAny<string>()), Times.Exactly(1));
            _mockHttpContextWrapper.Verify(x => x.SignOutAsync(), Times.Exactly(1));
            _mockHttpContextWrapper.Verify(x => x.SignInAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<ChangePasswordModel>(((ViewResult) result).Model);
            Assert.AreEqual("Your password has been changed.", ((ChangePasswordModel)((ViewResult)result).Model).StatusMessage);
        }

        private void SetupAuthenticationMock()
        {
            _mockHttpContextWrapper = new Mock<IHttpContextWrapper>();
            _mockHttpContextWrapper.Setup(x => x.SignInAsync(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()));
            _mockHttpContextWrapper.Setup(x => x.SignInAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()));
            _mockHttpContextWrapper.Setup(x => x.SignOutAsync(It.IsAny<string>()));
            _mockHttpContextWrapper.Setup(x => x.SignOutAsync());
        }

        private void SetupContextUser()
        {
            var identity = new Mock<IIdentity>()
            {
                Name = "simon.davall@gmail.com"
            };

            var userMock = new Mock<ClaimsPrincipal>(identity.Object);
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "user-id") };
            userMock.Setup(p => p.Claims).Returns(claims);

            var context = new DefaultHttpContext {User = userMock.Object};
            _controllerContext = new ControllerContext
            {
                HttpContext = context, ModelState = {  }
            };
        }

        private AuthenticateResult GetAuthenticateResult()
        {
            var testScheme = "FakeScheme";
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(new[] {
                new Claim("amr", "access-token"),
                new Claim("Manager", "yes"),
                new Claim(ClaimTypes.Role, "Administrator"),
                new Claim(ClaimTypes.NameIdentifier, "John")
            }, testScheme));

            return AuthenticateResult.Success(new AuthenticationTicket(principal, new AuthenticationProperties(), testScheme));
        }
    }
}
