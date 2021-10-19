using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.Models.Manage;

namespace PegasusApi.Tests.Controllers
{
    public class BaseControllerTest
    {
        protected Mock<UserManager<IdentityUser>> MockUserManager;
        protected Mock<ILogger<PegasusApi.Controllers.ManageController>> MockLogger;

        protected const string Username = "test.user@email.com";
        protected const string Password = "SecretPassword";
        protected const string UserId = "12345";
        protected const string PhoneNumber = "+441234675890";

        // bad data
        protected const string BadUserId = "BadValueId";
        protected const string BadUsername = "bad.user@email.com";
        protected const string BadPassword = "BadPassword";
        protected const string BadPhoneNumber = "+NotValid";

        protected static IdentityError TestError;

        protected IdentityUser User;

        [OneTimeSetUp]
        public virtual void OneTimeSetup()
        {
            TestError = new IdentityError {Code = "ErrorCode1002", Description = "An error has occurred."};
            User = new IdentityUser { Id = UserId, UserName = Username, PasswordHash = Password };
        }

        [SetUp]
        public virtual void EachTestSetUp()
        {
            MockUserManager = CreateMockUserManager(User);
            MockLogger = CreateMockLogger();
        }
        
        private static Mock<UserManager<TUser>> CreateMockUserManager<TUser>(TUser user) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Setup(x => x.FindByEmailAsync(Username)).ReturnsAsync(user);
            mgr.Setup(x => x.FindByIdAsync(UserId)).ReturnsAsync(user);
            mgr.Setup(x => x.FindByEmailAsync(BadUsername)).ReturnsAsync((TUser)null);
            mgr.Setup(x => x.FindByIdAsync(BadUserId)).ReturnsAsync((TUser)null);
            return mgr;
        }
        
        private static Mock<ILogger<PegasusApi.Controllers.ManageController>> CreateMockLogger()
        {
            var mockLogger = new Mock<ILogger<PegasusApi.Controllers.ManageController>>();
            mockLogger.Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), 
                It.Is<It.IsAnyType>((v, t) => true), It.IsAny<Exception>(), 
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
            return mockLogger;
        }

        //TODO Currently there is no error handling for database interaction. Need to add some to all database calls. Add to logging to.
        // protected static SqlException MakeSqlException() {
        //     SqlException exception = null;
        //     try 
        //     {
        //         var conn = new SqlConnection(@"Data Source=.;Database=GUARANTEED_TO_FAIL;Connection Timeout=1");
        //         conn.Open();
        //     } 
        //     catch(SqlException ex) 
        //     {
        //         exception = ex;
        //     }
        //     return(exception);
        // }
        
        protected void VerifyErrorLogged(Times times)
        {
            MockLogger.Verify(x => x.Log(LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), times);
        }

        protected static void AssertHasErrors<T>(T result, int count) where T : ManageBaseModel
        {
            result.Should().NotBeNull();
            result.Errors.Should().NotBeNull();
            result.Errors.Count.Should().Be(count);
        }
        
        protected async Task CaseModelIsNull<T>(Func<T, Task<T>> methodToTest) 
            where T : ManageBaseModel
        {
            MockLogger.Invocations.Clear();
            var result1 = await methodToTest(null);

            VerifyErrorLogged(Times.Never());
            result1.Should().BeOfType<T>();
            AssertHasErrors(result1, 1);
        }
        
        protected async Task CaseUserIdIsNull<T>(Func<string, Task<T>> methodToTest) where T : ManageBaseModel
        {
            MockLogger.Invocations.Clear();
            var result1 = await methodToTest(null);

            VerifyErrorLogged(Times.Once());
            result1.Should().BeOfType<T>();
            AssertHasErrors(result1, 1);
        }

        protected async Task CaseUserIdIsNull<T>(Func<T, Task<T>> methodToTest) 
            where T : ManageBaseModel, new()
        {
            MockLogger.Invocations.Clear();
            var result1 = await methodToTest(new T { UserId = null });

            VerifyErrorLogged(Times.Once());
            result1.Should().BeOfType<T>();
            AssertHasErrors(result1, 1);
        }
        
        protected async Task CaseUserNotFound<T>(Func<string, Task<T>> methodToTest) where T : ManageBaseModel
        {
            MockLogger.Invocations.Clear();
            var result1 = await methodToTest(BadUserId);

            VerifyErrorLogged(Times.Once());
            result1.Should().BeOfType<T>();
            AssertHasErrors(result1, 1);
        }
        
        protected async Task CaseUserNotFound<T>(Func<T, Task<T>> methodToTest) 
            where T : ManageBaseModel, new()
        {
            MockLogger.Invocations.Clear();
            var result1 = await methodToTest(new T { UserId = BadUserId });

            VerifyErrorLogged(Times.Once());
            result1.Should().BeOfType<T>();
            AssertHasErrors(result1, 1);
        }
    }
}
