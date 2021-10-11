using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Library.Api;
using Pegasus.Services;

namespace PegasusTests.Controllers.AccountControllerTests
{
    class AccountControllerTestsBase
    {
        protected IUrlHelper Url;
        protected Mock<ILogger<AccountController>> _mockLogger;
        protected Mock<IApiHelper> _mockApiHelper;
        protected Mock<IAccountsEndpoint> _mockAccountsEndpoint;
        protected Mock<IAuthenticationEndpoint> _mockAuthenticationEndpoint;
        protected Mock<ISignInManager> _mockSignInManager;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _mockAuthenticationEndpoint = new Mock<IAuthenticationEndpoint>();
            _mockLogger = new Mock<ILogger<AccountController>>();
            _mockSignInManager = new Mock<ISignInManager>();
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x => x.IsLocalUrl(It.IsAny<string>())).Returns(true);
            mockUrlHelper.Setup(x => x.IsLocalUrl(string.Empty)).Returns(false);
            mockUrlHelper.Setup(x => x.IsLocalUrl(null)).Returns(false);
            Url = mockUrlHelper.Object;
        }

        [SetUp]
        public void EachTestSetup()
        {
            _mockAccountsEndpoint = new Mock<IAccountsEndpoint>();
            _mockApiHelper = new Mock<IApiHelper>();
        }
    }
}
