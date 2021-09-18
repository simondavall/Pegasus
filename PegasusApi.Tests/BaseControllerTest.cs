using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Tests
{
    public class BaseControllerTest
    {
        protected UserManager<IdentityUser> _userManager;

        protected static readonly string _username = "test.user@email.com";
        protected static readonly string _password = "SecretPassword";
        protected static readonly string _userId = "12345";
        protected static readonly string _phoneNumber = "+441234675890";
        
        // bad data
        protected static readonly string _badUserId = "BadValueId";
        protected static readonly string _badUsername = "bad.user@email.com";
        protected static readonly string _badPassword = "BadPassword";
        protected static readonly string _badPhoneNumber = "+NotValid";
        protected static readonly string _badDisplayName = "Bad Test User";

        protected static IdentityError _testError;

        protected IdentityUser _user;
        protected static UserModel _badUserModel;

        [OneTimeSetUp]
        public virtual void OneTimeSetup()
        {
            _testError = new IdentityError {Code = "ErrorCode1002", Description = "An error has occurred."};
            _user = new IdentityUser { Id = _userId, UserName = _username, PasswordHash = _password };
            _badUserModel = new UserModel { DisplayName = _badDisplayName, Id = _userId };
            _userManager = MockUserManager(_user).Object;
        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>(TUser user) where TUser : class
        {
            var identityErrors = new[]{_testError};

            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            // set up for success
            mgr.Setup(x => x.FindByEmailAsync(_username)).ReturnsAsync(user);
            mgr.Setup(x => x.FindByIdAsync(_userId)).ReturnsAsync(user);
            mgr.Setup(x => x.CheckPasswordAsync(It.IsAny<TUser>(), _password)).ReturnsAsync(true);
            mgr.Setup(x => x.GetPhoneNumberAsync(It.IsAny<TUser>())).ReturnsAsync(_phoneNumber);
            mgr.Setup(x => x.SetPhoneNumberAsync(It.IsAny<TUser>(), _phoneNumber)).ReturnsAsync(IdentityResult.Success);
            // set up for fail
            mgr.Setup(x => x.FindByEmailAsync(_badUsername)).ReturnsAsync((TUser) null);
            mgr.Setup(x => x.FindByIdAsync(_badUserId)).ReturnsAsync((TUser) null);
            mgr.Setup(x => x.CheckPasswordAsync(It.IsAny<TUser>(), _badPassword)).ReturnsAsync(false);
            mgr.Setup(x => x.SetPhoneNumberAsync(It.IsAny<TUser>(), _badPhoneNumber)).ReturnsAsync(IdentityResult.Failed(identityErrors));

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
