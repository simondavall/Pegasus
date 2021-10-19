using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Moq;
using NUnit.Framework;

namespace PegasusApi.Tests.Controllers.AccountController
{
    public class AccountControllerTestsBase
    {
        protected Mock<IEmailSender> MockEmailSender;
        protected Mock<UserManager<IdentityUser>> MockUserManager;
        protected const string EmailAddressGood = "test-good@email.com";
        protected const string EmailAddressNotFound = "test-bad@email.com";
        protected const string UserId = "user-id";
        protected const string UnknownUserId = "unknown-id";

        [SetUp]
        public void EachTestSetup()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            MockUserManager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
            MockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);
            MockUserManager.Setup(x => x.FindByEmailAsync( EmailAddressGood)).ReturnsAsync(new IdentityUser { Id = UserId });
            MockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);
            MockUserManager.Setup(x => x.FindByIdAsync(UserId)).ReturnsAsync(new IdentityUser { Id=UserId, SecurityStamp = "security-stamp" });
            MockEmailSender = new Mock<IEmailSender>();
        }
        
        internal PegasusApi.Controllers.AccountController CreateAccountController()
        {
            return new PegasusApi.Controllers.AccountController(MockUserManager.Object, MockEmailSender.Object);
        }

    }
}