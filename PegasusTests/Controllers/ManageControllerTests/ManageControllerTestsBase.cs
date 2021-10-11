﻿using System.Collections.Generic;
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

namespace PegasusTests.Controllers.ManageControllerTests
{
    class ManageControllerTestsBase
    {
        protected const string UserId = "user-id";
        protected ControllerContext _controllerContext;
        protected Mock<IHttpContextWrapper> _mockHttpContextWrapper;
        protected Mock<IAccountsEndpoint> _mockAccountsEndpoint;
        protected Mock<IApiHelper> _mockApiHelper;
        protected Mock<IJwtTokenAccessor> _mockTokenAccessor;
        protected Mock<IAuthenticationEndpoint> _mockAuthenticationEndpoint;
        protected Mock<ILogger<Pegasus.Controllers.ManageController>> _logger;

        protected static IEnumerable<IdentityError> TestErrors => new List<IdentityError>
            { new IdentityError { Code = "ErrorCode", Description = "Error Message" } };

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

        protected Pegasus.Controllers.ManageController CreateManageController()
        {
            var manageEndpoint = new ManageEndpoint(_mockApiHelper.Object);

            var signInManager = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object,
                _mockApiHelper.Object, _mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

            var sut = new Pegasus.Controllers.ManageController(manageEndpoint, signInManager, _logger.Object)
            {
                ControllerContext = _controllerContext
            };
            return sut;
        }

        private void SetupAuthenticationMock()
        {
            _mockHttpContextWrapper = new Mock<IHttpContextWrapper>();
            _mockHttpContextWrapper.Setup(x => x.SignInAsync(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()));
            _mockHttpContextWrapper.Setup(x => x.SignInAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()));
            _mockHttpContextWrapper.Setup(x => x.SignOutAsync(It.IsAny<string>()));
            _mockHttpContextWrapper.Setup(x => x.SignOutAsync());
        }

        protected void SetupSignInMocks()
        {
            _mockAuthenticationEndpoint.Setup(x => x.Authenticate(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatedUser { UserId = UserId });
            _mockAuthenticationEndpoint.Setup(x => x.Authenticate2Fa(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatedUser { UserId = UserId });
            _mockTokenAccessor.Setup(x => x.GetAccessTokenWithClaimsPrincipal(It.IsAny<AuthenticatedUser>()))
                .Returns(new TokenWithClaimsPrincipal());
            _mockAccountsEndpoint.Setup(x => x.RememberClientAsync(It.IsAny<string>())).ReturnsAsync(new RememberClientModel
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