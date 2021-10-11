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
        }

        [SetUp]
        public void EachTestSetup()
        {
            _mockAccountsEndpoint = new Mock<IAccountsEndpoint>();
            _mockApiHelper = new Mock<IApiHelper>();
        }
    }
}
