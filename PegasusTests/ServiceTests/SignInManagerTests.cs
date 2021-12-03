using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using Pegasus.Library.Api;
using Pegasus.Library.JwtAuthentication;
using Pegasus.Library.JwtAuthentication.Models;
using Pegasus.Library.Models.Account;
using Pegasus.Library.Services.Http;
using Pegasus.Models.Account;
using Pegasus.Services;


namespace PegasusTests.ServiceTests
{
    class SignInManagerTests
    {
        private Mock<IHttpContextWrapper> _mockHttpContextWrapper;
        private Mock<IAccountsEndpoint> _mockAccountsEndpoint;
        private Mock<IApiHelper> _mockApiHelper;
        private Mock<IJwtTokenAccessor> _mockTokenAccessor;
        private Mock<IAuthenticationEndpoint> _mockAuthenticationEndpoint;

        [SetUp]
        public void TestSetup()
        {
            _mockApiHelper = new Mock<IApiHelper>();
            _mockTokenAccessor = new Mock<IJwtTokenAccessor>();
            _mockAuthenticationEndpoint = new Mock<IAuthenticationEndpoint>();
            _mockHttpContextWrapper = new Mock<IHttpContextWrapper>();
            _mockAccountsEndpoint = new Mock<IAccountsEndpoint>();
            SetupAuthenticationMock();
        }

        [Test]
        public async Task DoTwoFactorSignInAsync_GoodUserIdWithRemember_CallsSignInTwice()
        {
            SetupSignInMocks();

            var sut = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object, _mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            await sut.DoTwoFactorSignInAsync("userId", true);

            _mockHttpContextWrapper.Verify(x => x.SignInAsync(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
            _mockHttpContextWrapper.Verify(x => x.SignOutAsync(), Times.Exactly(1));
            _mockHttpContextWrapper.Verify(x => x.SignInAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
        }

        [Test]
        public async Task DoTwoFactorSignInAsync_GoodUserIdWithoutRemember_CallsSignInOne()
        {
            SetupSignInMocks();

            var sut = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            await sut.DoTwoFactorSignInAsync("userId", false);

            _mockAccountsEndpoint.Verify(x => x.RememberClientAsync(It.IsAny<string>()), Times.Never);
            _mockHttpContextWrapper.Verify(x => x.SignOutAsync(), Times.Exactly(1));
            _mockHttpContextWrapper.Verify(x => x.SignInAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
        }

        [Test]
        public async Task DoSignInAsync_GoodUserIdWithRemember_CallsSignInTwice()
        {
            SetupSignInMocks();

            var sut = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            await sut.DoSignInAsync("userId", true);

            _mockHttpContextWrapper.Verify(x => x.SignInAsync(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
            _mockHttpContextWrapper.Verify(x => x.SignOutAsync(), Times.Exactly(1));
            _mockHttpContextWrapper.Verify(x => x.SignInAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
        }

        [Test]
        public async Task DoSignInAsync_GoodUserIdWithoutRemember_CallsSignInOne()
        {
            SetupSignInMocks();

            var sut = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            await sut.DoSignInAsync("userId", false);

            _mockAccountsEndpoint.Verify(x => x.RememberClientAsync(It.IsAny<string>()), Times.Never);
            _mockHttpContextWrapper.Verify(x => x.SignOutAsync(), Times.Exactly(1));
            _mockHttpContextWrapper.Verify(x => x.SignInAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
        }

        [Test]
        public async Task GetTwoFactorAuthenticationUserAsync_Success_ReturnsUserId()
        {
            _mockHttpContextWrapper.Setup(x => x.AuthenticateAsync(It.IsAny<string>()))
                .ReturnsAsync(GetSuccessAuthenticateResult());

            var sut = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            var result = await sut.GetTwoFactorAuthenticationUserAsync();

            _mockHttpContextWrapper.Verify(x => x.AuthenticateAsync(It.IsAny<string>()), Times.Exactly(1));
            Assert.AreEqual("user-id", result);
        }

        [Test]
        public async Task GetTwoFactorAuthenticationUserAsync_Fail_ReturnsNull()
        {
            _mockHttpContextWrapper.Setup(x => x.AuthenticateAsync(It.IsAny<string>()))
                .ReturnsAsync(AuthenticateResult.Fail(string.Empty));

            var sut = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            var result = await sut.GetTwoFactorAuthenticationUserAsync();

            _mockHttpContextWrapper.Verify(x => x.AuthenticateAsync(It.IsAny<string>()), Times.Exactly(1));
            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task SignInOrTwoFactor_GoodCredentialsTwoFactorNotRequired_ReturnsSuccess()
        {
            SetupSignInMocks();

            var sut = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            var result = await sut.SignInOrTwoFactor(new AuthenticatedUser());

            Assert.IsInstanceOf<SignInResultModel>(result);
            Assert.AreEqual(true, result.Success);
        }

        [Test]
        public async Task SignInOrTwoFactor_GoodCredentialsTwoFactorRequiredRemembered_ReturnsSuccess()
        {
            SetupSignInMocks();
            var testTokenWithClaimsPrincipal = new TokenWithClaimsPrincipal()
            {
                AccessToken = "access-token",
                ClaimsPrincipal = TestClaimsPrincipal,
                AuthenticationProperties = null
            };

            _mockHttpContextWrapper.Setup(x => x.AuthenticateAsync(It.IsAny<string>())).ReturnsAsync(GetSuccessAuthenticateResult());
            _mockHttpContextWrapper.Setup(x => x.SignInAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()));
            _mockTokenAccessor.Setup(x => x.GetAccessTokenWithClaimsPrincipal(It.IsAny<AuthenticatedUser>())).Returns(testTokenWithClaimsPrincipal);

            var sut = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            var result = await sut.SignInOrTwoFactor(new AuthenticatedUser {RequiresTwoFactor = true, UserId = "user-id"});

            _mockHttpContextWrapper.Verify(x => x.AuthenticateAsync(It.IsAny<string>()), Times.Exactly(1));
            _mockHttpContextWrapper.Verify(x =>
                x.SignInAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
            Assert.IsTrue(result.Success, "Result check failed.");
        }

        [Test]
        public async Task SignInOrTwoFactor_GoodCredentialsTwoFactorRequired_ReturnsRequiresTwoFactor()
        {
            SetupSignInMocks();
            var testTokenWithClaimsPrincipal = new TokenWithClaimsPrincipal()
            {
                AccessToken = "access-token",
                ClaimsPrincipal = TestClaimsPrincipal,
                AuthenticationProperties = null
            };


            _mockHttpContextWrapper.Setup(x => x.AuthenticateAsync(It.IsAny<string>())).ReturnsAsync(AuthenticateResult.Fail(string.Empty));
            _mockHttpContextWrapper.Setup(x => x.SignInAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()));
            _mockTokenAccessor.Setup(x => x.GetAccessTokenWithClaimsPrincipal(It.IsAny<AuthenticatedUser>())).Returns(testTokenWithClaimsPrincipal);

            var sut = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            var result = await sut.SignInOrTwoFactor(new AuthenticatedUser {RequiresTwoFactor = true, UserId = "user-id"});

            _mockHttpContextWrapper.Verify(x => x.AuthenticateAsync(It.IsAny<string>()), Times.Exactly(1));
            _mockHttpContextWrapper.Verify(x => x.SignInAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Never);
            _mockHttpContextWrapper.Verify(x => x.SignInAsync(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>()), Times.Exactly(1));
            Assert.IsTrue(result.RequiresTwoFactor, "Result check failed.");
        }

        [Test]
        public async Task SignOut_ExecutesSignOut()
        {
            var sut = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            await sut.SignOutAsync(string.Empty);

            _mockHttpContextWrapper.Verify(x => x.SignOutAsync(It.IsAny<string>()), Times.Exactly(1));
        }

        [Test]
        public async Task TwoFactorRecoveryCodeSignInAsync_NoUserId_ReturnsFailedSignIn()
        {
            _mockHttpContextWrapper.Setup(x => x.AuthenticateAsync(It.IsAny<string>()))
                .ReturnsAsync(AuthenticateResult.Fail(string.Empty));

            var sut = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            var result = await sut.TwoFactorRecoveryCodeSignInAsync(string.Empty);

            Assert.IsInstanceOf<SignInResult>(result);
            Assert.AreEqual(SignInResult.Failed, result);
        }

        [Test]
        public async Task TwoFactorRecoveryCodeSignInAsync_RecoverCodeFailed_ReturnsFailedSignIn()
        {
            _mockHttpContextWrapper.Setup(x => x.AuthenticateAsync(It.IsAny<string>()))
                .ReturnsAsync(GetSuccessAuthenticateResult());
            _mockAccountsEndpoint
                .Setup(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<RedeemTwoFactorRecoveryCodeModel>()))
                .ReturnsAsync(new RedeemTwoFactorRecoveryCodeModel { Succeeded = false });

            var sut = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            var result = await sut.TwoFactorRecoveryCodeSignInAsync(string.Empty);

            Assert.IsInstanceOf<SignInResult>(result);
            Assert.AreEqual(SignInResult.Failed, result);
        }

        [Test]
        public async Task TwoFactorRecoveryCodeSignInAsync_RecoverCodeSuccess_ReturnsSignInSuccess()
        {
            SetupAuthenticationMock();
            SetupSignInMocks();

            _mockHttpContextWrapper.Setup(x => x.AuthenticateAsync(It.IsAny<string>()))
                .ReturnsAsync(GetSuccessAuthenticateResult());
            _mockAccountsEndpoint
                .Setup(x => x.RedeemTwoFactorRecoveryCodeAsync(It.IsAny<RedeemTwoFactorRecoveryCodeModel>()))
                .ReturnsAsync(new RedeemTwoFactorRecoveryCodeModel { Succeeded = true });
            _mockTokenAccessor.Setup(x => x.GetAccessTokenWithClaimsPrincipal(It.IsAny<AuthenticatedUser>())).Returns(new TokenWithClaimsPrincipal());

            var sut = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);
            var result = await sut.TwoFactorRecoveryCodeSignInAsync(string.Empty);

            _mockAuthenticationEndpoint.Verify(x => x.Authenticate2Fa(It.IsAny<string>()), Times.Exactly(1));
            _mockTokenAccessor.Verify(x => x.GetAccessTokenWithClaimsPrincipal(It.IsAny<AuthenticatedUser>()), Times.Exactly(1));
            _mockHttpContextWrapper.Verify(x => x.SignOutAsync(), Times.Exactly(1));
            _mockHttpContextWrapper.Verify(x => x.SignInAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Exactly(1));
            Assert.IsInstanceOf<SignInResult>(result);
            Assert.AreEqual(SignInResult.Success, result);
        }



        private void SetupAuthenticationMock()
        {
            _mockHttpContextWrapper = new Mock<IHttpContextWrapper>();
            _mockHttpContextWrapper.Setup(x => x.SignInAsync(It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()));
            _mockHttpContextWrapper.Setup(x => x.SignInAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()));
            _mockHttpContextWrapper.Setup(x => x.SignOutAsync(It.IsAny<string>()));
            _mockHttpContextWrapper.Setup(x => x.SignOutAsync());
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

        private AuthenticateResult GetSuccessAuthenticateResult()
        {
            const string testScheme = "FakeScheme";
            return AuthenticateResult.Success(new AuthenticationTicket(TestClaimsPrincipal, new AuthenticationProperties(), testScheme));
        }

        private ClaimsPrincipal TestClaimsPrincipal
        {
            get
            {
                const string testScheme = "FakeScheme";
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user"),
                    new Claim(ClaimTypes.Name, "user-id"),
                    new Claim(ClaimTypes.AuthenticationMethod, "access-token")
                };
                var claimsIdentity = new ClaimsIdentity(claims, testScheme);

                var principal = new ClaimsPrincipal();
                principal.AddIdentity(claimsIdentity);
                return principal;
            }
        }
    }
}

