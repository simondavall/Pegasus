using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Pegasus.Library.Api;
using Pegasus.Library.JwtAuthentication;
using Pegasus.Library.JwtAuthentication.Models;
using Pegasus.Library.Models.Account;
using Pegasus.Services;

namespace PegasusTests.ServiceTests
{
    class SignInManagerTests
    {
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<IAccountsEndpoint> _mockAccountsEndpoint;
        private Mock<IApiHelper> _mockApiHelper;
        private Mock<IJwtTokenAccessor> _mockTokenAccessor;
        private Mock<IAuthenticationEndpoint> _mockAuthenticationEndpoint;
        private Mock<IAuthenticationService> _authServiceMock;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockApiHelper = new Mock<IApiHelper>();
            _mockTokenAccessor = new Mock<IJwtTokenAccessor>();
            _mockAuthenticationEndpoint = new Mock<IAuthenticationEndpoint>();
        }

        [SetUp]
        public void TestSetup()
        {
            _mockAccountsEndpoint = new Mock<IAccountsEndpoint>();
            SetupAuthenticationMock();
        }

        [Test]
        public async Task DoTwoFactorSignInAsync_GoodUserIdWithRemember_CallsSignInTwice()
        {
            SetupSignInMocks();

            var sut = new SignInManager(_mockHttpContextAccessor.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object, _mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            await sut.DoTwoFactorSignInAsync("userId", true);

            _authServiceMock.Verify(x => x.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
            _authServiceMock.Verify(x => x.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(2));
        }

        [Test]
        public async Task DoTwoFactorSignInAsync_GoodUserIdWithoutRemember_CallsSignInOne()
        {
            SetupSignInMocks();

            var sut = new SignInManager(_mockHttpContextAccessor.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            await sut.DoTwoFactorSignInAsync("userId", false);

            _mockAccountsEndpoint.Verify(x => x.RememberClientAsync(It.IsAny<string>()), Times.Never);
            _authServiceMock.Verify(x => x.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
            _authServiceMock.Verify(x => x.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
        }

        [Test]
        public async Task DoSignInAsync_GoodUserIdWithRemember_CallsSignInTwice()
        {
            SetupSignInMocks();

            var sut = new SignInManager(_mockHttpContextAccessor.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            await sut.DoSignInAsync("userId", true);

            _authServiceMock.Verify(x => x.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
            _authServiceMock.Verify(x => x.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(2));
        }

        [Test]
        public async Task DoSignInAsync_GoodUserIdWithoutRemember_CallsSignInOne()
        {
            SetupSignInMocks();

            var sut = new SignInManager(_mockHttpContextAccessor.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            await sut.DoSignInAsync("userId", false);

            _mockAccountsEndpoint.Verify(x => x.RememberClientAsync(It.IsAny<string>()), Times.Never);
            _authServiceMock.Verify(x => x.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
            _authServiceMock.Verify(x => x.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
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
        private void SetupSignInMocks()
        {
            _mockAuthenticationEndpoint.Setup(x => x.Authenticate(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatedUser { UserId = "user-id" });
            _mockAuthenticationEndpoint.Setup(x => x.Authenticate2Fa(It.IsAny<string>()))
                .ReturnsAsync(new AuthenticatedUser { UserId = "user-id" });
            _mockTokenAccessor.Setup(x => x.GetAccessTokenWithClaimsPrincipal(It.IsAny<AuthenticatedUser>()))
                .Returns(new TokenWithClaimsPrincipal());
            _mockAccountsEndpoint.Setup(x => x.RememberClientAsync(It.IsAny<string>())).ReturnsAsync(new RememberClientModel
                { SupportsUserSecurityStamp = true, SecurityStamp = "security-stamp" });
        }
    }
}
