using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Tests
{
    public class BaseControllerTest
    {
        protected UserManager<IdentityUser> UserManager;

        protected const string Username = "test.user@email.com";
        protected const string Password = "SecretPassword";
        protected const string UserId = "12345";
        protected const string PhoneNumber = "+441234675890";

        // bad data
        protected const string BadUserId = "BadValueId";
        protected const string BadUsername = "bad.user@email.com";
        protected const string BadPassword = "BadPassword";
        protected const string BadPhoneNumber = "+NotValid";
        protected const string BadDisplayName = "Bad Test User";

        protected static IdentityError TestError;

        protected IdentityUser User;
        protected static UserModel BadUserModel;

        [OneTimeSetUp]
        public virtual void OneTimeSetup()
        {
            TestError = new IdentityError {Code = "ErrorCode1002", Description = "An error has occurred."};
            User = new IdentityUser { Id = UserId, UserName = Username, PasswordHash = Password };
            BadUserModel = new UserModel { DisplayName = BadDisplayName, Id = UserId };
            UserManager = MockUserManager(User).Object;
        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>(TUser user) where TUser : class
        {
            var identityErrors = new[]{TestError};

            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            // set up for success
            mgr.Setup(x => x.FindByEmailAsync(Username)).ReturnsAsync(user);
            mgr.Setup(x => x.FindByIdAsync(UserId)).ReturnsAsync(user);
            mgr.Setup(x => x.CheckPasswordAsync(It.IsAny<TUser>(), Password)).ReturnsAsync(true);
            mgr.Setup(x => x.GetPhoneNumberAsync(It.IsAny<TUser>())).ReturnsAsync(PhoneNumber);
            mgr.Setup(x => x.SetPhoneNumberAsync(It.IsAny<TUser>(), PhoneNumber)).ReturnsAsync(IdentityResult.Success);
            // set up for fail
            mgr.Setup(x => x.FindByEmailAsync(BadUsername)).ReturnsAsync((TUser) null);
            mgr.Setup(x => x.FindByIdAsync(BadUserId)).ReturnsAsync((TUser) null);
            mgr.Setup(x => x.CheckPasswordAsync(It.IsAny<TUser>(), BadPassword)).ReturnsAsync(false);
            mgr.Setup(x => x.SetPhoneNumberAsync(It.IsAny<TUser>(), BadPhoneNumber)).ReturnsAsync(IdentityResult.Failed(identityErrors));

            return mgr;
        }


        //TODO Currently there is no error handling for database interaction. Need to add some to all database calls. Add to logging to.
        protected static SqlException MakeSqlException() {
            SqlException exception = null;
            try 
            {
                var conn = new SqlConnection(@"Data Source=.;Database=GUARANTEED_TO_FAIL;Connection Timeout=1");
                conn.Open();
            } 
            catch(SqlException ex) 
            {
                exception = ex;
            }
            return(exception);
        }
    }
}
