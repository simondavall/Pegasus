using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
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
using Pegasus.Library.Models.Account;
using Pegasus.Library.Services.Http;
using Pegasus.Services;

namespace Pegasus.Integration.Tests.Controllers.ManageControllerTests
{
    class ManageControllerTestsBase
    {
        protected const string UserId = "user-id";
        protected ControllerContext ControllerContext;
        protected Mock<IHttpContextWrapper> MockHttpContextWrapper;
        protected Mock<IAccountsEndpoint> MockAccountsEndpoint;
        protected Mock<IApiHelper> MockApiHelper;
        protected Mock<IJwtTokenAccessor> MockTokenAccessor;
        protected Mock<IAuthenticationEndpoint> MockAuthenticationEndpoint;
        protected Mock<ILogger<Pegasus.Controllers.ManageController>> Logger;

        protected static IEnumerable<IdentityError> TestErrors => new List<IdentityError>
            { new IdentityError { Code = "ErrorCode", Description = "Error Message" } };

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            MockHttpContextWrapper = new Mock<IHttpContextWrapper>();
            MockTokenAccessor = new Mock<IJwtTokenAccessor>();
            MockAuthenticationEndpoint = new Mock<IAuthenticationEndpoint>();
            Logger = new Mock<ILogger<Pegasus.Controllers.ManageController>>();
        }

        [SetUp]
        public void TestSetup()
        {
            MockApiHelper = new Mock<IApiHelper>();
            MockAccountsEndpoint = new Mock<IAccountsEndpoint>();
            SetupAuthenticationMock();
            SetupContextUser();
        }

        protected Pegasus.Controllers.ManageController CreateManageController()
        {
            var manageEndpoint = new ManageEndpoint(MockApiHelper.Object);

            var signInManager = new SignInManager(MockHttpContextWrapper.Object, MockAccountsEndpoint.Object,
                MockApiHelper.Object, MockTokenAccessor.Object, MockAuthenticationEndpoint.Object);

            var sut = new Pegasus.Controllers.ManageController(manageEndpoint, signInManager, Logger.Object)
            {
                ControllerContext = ControllerContext
            };
            return sut;
        }

        private void SetupAuthenticationMock()
        {
            MockHttpContextWrapper = new Mock<IHttpContextWrapper>();
            MockHttpContextWrapper.Setup(x => x.SignInAsync(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()));
            MockHttpContextWrapper.Setup(x => x.SignInAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()));
            MockHttpContextWrapper.Setup(x => x.SignOutAsync(It.IsAny<string>()));
            MockHttpContextWrapper.Setup(x => x.SignOutAsync());
        }

        protected void SetupSignInMocks()
        {
            MockAuthenticationEndpoint.Setup(x => x.Authenticate(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatedUser { UserId = UserId });
            MockAuthenticationEndpoint.Setup(x => x.Authenticate2Fa(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatedUser { UserId = UserId });
            MockTokenAccessor.Setup(x => x.GetAccessTokenWithClaimsPrincipal(It.IsAny<AuthenticatedUser>()))
                .Returns(new TokenWithClaimsPrincipal());
            MockAccountsEndpoint.Setup(x => x.RememberClientAsync(It.IsAny<string>())).ReturnsAsync(new RememberClientModel
                { SupportsUserSecurityStamp = true, SecurityStamp = "security-stamp" });
        }

        private void SetupContextUser()
        {
            var identity = new Mock<IIdentity>()
            {
                Name = "simon.davall@gmail.com"
            };

            var userMock = new Mock<ClaimsPrincipal>(identity.Object);
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, UserId) };
            userMock.Setup(p => p.Claims).Returns(claims);

            var context = new DefaultHttpContext {User = userMock.Object};
            ControllerContext = new ControllerContext
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
