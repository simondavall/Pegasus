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
        protected Mock<ILogger<AccountController>> MockLogger;
        protected Mock<IApiHelper> MockApiHelper;
        protected Mock<IAccountsEndpoint> MockAccountsEndpoint;
        protected Mock<IAuthenticationEndpoint> MockAuthenticationEndpoint;
        protected Mock<ISignInManager> MockSignInManager;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            MockAuthenticationEndpoint = new Mock<IAuthenticationEndpoint>();
            MockLogger = new Mock<ILogger<AccountController>>();
            MockSignInManager = new Mock<ISignInManager>();
        }

        [SetUp]
        public void EachTestSetup()
        {
            MockAccountsEndpoint = new Mock<IAccountsEndpoint>();
            MockApiHelper = new Mock<IApiHelper>();
        }
    }
}
