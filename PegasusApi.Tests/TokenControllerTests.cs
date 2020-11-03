using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using PegasusApi.Controllers;
using PegasusApi.Data;
using PegasusApi.Models;

namespace PegasusApi.Tests
{
    public class TokenControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _options;
        private UserManager<IdentityUser> _userManager;
        private string _username = "test.user@email.com";
        private string _password = "SecretPassword";
        private IConfiguration _configuration;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var jsonConfig = "{ \"Token\": { \"Issuer\": \"https://localhost:5001\", \"Audience\": \"https://localhost:5002\", \"SigningKey\": \"MySecretKeyIsSecretSoDoNotTell\" }}";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonConfig));

            _configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            var user1 = new IdentityUser { Id = "12345", UserName = _username, PasswordHash = _password };

            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "PegasusAuth")
                .Options;

            using (var context = new ApplicationDbContext(_options))
            {
                context.Users.Add(user1);
                context.SaveChanges();
            }

            _userManager = MockUserManager(new List<IdentityUser> { user1 }).Object;
        }

        [Test]
        public void TokenControllerTest()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                var tokenController = new TokenController(context, _userManager, _configuration);

                var sut = tokenController.Create(_username, _password, "password").Result;

                Assert.IsInstanceOf<ObjectResult>(sut);

                dynamic tokenObject = ((ObjectResult) sut).Value;
                Assert.IsInstanceOf<TokenModel>(tokenObject);
                Assert.AreEqual(_username, tokenObject.Username);
            }
        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> users) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => users.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(users[0]);
            mgr.Setup(x => x.CheckPasswordAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(true);

            return mgr;
        }
    }
}