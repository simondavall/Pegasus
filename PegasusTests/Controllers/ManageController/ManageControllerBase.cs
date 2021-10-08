using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Pegasus.Library.Api;
using Pegasus.Library.JwtAuthentication;
using Pegasus.Library.Services.Http;

namespace PegasusTests.Controllers.ManageController
{
    class ManageControllerBase
    {
        protected ControllerContext _controllerContext;
        protected Mock<IHttpContextWrapper> _mockHttpContextWrapper;
        protected Mock<IAccountsEndpoint> _mockAccountsEndpoint;
        protected Mock<IApiHelper> _mockApiHelper;
        protected Mock<IJwtTokenAccessor> _mockTokenAccessor;
        protected Mock<IAuthenticationEndpoint> _mockAuthenticationEndpoint;
        protected Mock<ILogger<Pegasus.Controllers.ManageController>> _logger;

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
                HttpContext = context
            };
        }

        protected AuthenticateResult GetAuthenticateResult()
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
