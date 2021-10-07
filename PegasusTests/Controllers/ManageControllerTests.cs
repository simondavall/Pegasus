using System;
using System.Collections.Generic;
using System.Net.Sockets;
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
using Pegasus.Library.Models.Manage;
using Pegasus.Services;

namespace PegasusTests.Controllers
{
    class ManageControllerTests
    {
        private ControllerContext _controllerContext;

        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<IAccountsEndpoint> _mockAccountsEndpoint;
        private Mock<IApiHelper> _mockApiHelper;
        private Mock<IJwtTokenAccessor> _mockTokenAccessor;
        private Mock<IAuthenticationEndpoint> _mockAuthenticationEndpoint;
        private Mock<IAuthenticationService> _authServiceMock;
        private Mock<ILogger<Pegasus.Controllers.ManageController>> _logger;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
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

            var signInManager = new SignInManager(_mockHttpContextAccessor.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

            var sut = new Pegasus.Controllers.ManageController(manageEndpoint, signInManager, _logger.Object)
            {
                ControllerContext = _controllerContext
            };

            var result = await sut.TwoFactorAuthentication(new TwoFactorAuthenticationModel());

            _authServiceMock.Verify(x => x.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
            _mockApiHelper.Verify(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task ChangePassword_HasErrors_ReturnsViewResult()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<HasPasswordModel>(It.IsAny<string>()))
                .ReturnsAsync(new HasPasswordModel {Errors = new List<IdentityError> {new IdentityError {Description = "Error Message"}}, StatusMessage = "Error"});
            var manageEndpoint = new ManageEndpoint(_mockApiHelper.Object);

            var signInManager = new SignInManager(_mockHttpContextAccessor.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

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

            var signInManager = new SignInManager(_mockHttpContextAccessor.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

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

            var signInManager = new SignInManager(_mockHttpContextAccessor.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object, _mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

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
            var signInManager = new SignInManager(_mockHttpContextAccessor.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object, _mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

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

            var signInManager = new SignInManager(_mockHttpContextAccessor.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

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
        public void POST_ChangePassword_NoErrors_ReturnsViewResultWithErrorInModelState()
        {

        }

        private void SetupAuthenticationMock()
        {
            _authServiceMock = new Mock<IAuthenticationService>();
            _authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            _authServiceMock
                .Setup(_ => _.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(_authServiceMock.Object);

            _mockHttpContextAccessor.Setup(x => x.HttpContext.RequestServices).Returns(serviceProviderMock.Object);
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


    }
}
