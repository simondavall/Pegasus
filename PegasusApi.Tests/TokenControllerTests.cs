using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PegasusApi.Controllers;
using PegasusApi.Data;
using PegasusApi.Library.JwtAuthentication;
using PegasusApi.Models;
using PegasusApi.Services;
using TokenOptions = PegasusApi.Library.JwtAuthentication.Models.TokenOptions;

namespace PegasusApi.Tests
{
    //TODO Need to implement this test for new JwtBearer version (3.1.18).

    public class TokenControllerTests
    {
        private IApplicationUserManager<IdentityUser> _userManager;
        private IApplicationDbContext _applicationDbContext;
        private static readonly string _username = "test.user@email.com";
        private static readonly string _password = "SecretPassword";
        private readonly string _issuer = "https://fakelocalhost:5001";
        private readonly string _audience = "https://fakelocalhost:5002";
        private readonly string _signingKey = "MySecretKeyIsSecretSoDoNotTell";
        private readonly string _tokenExpiryInMinute = "720";
        private static readonly string _userId = "12345";
        
        // bad data
        private static readonly string _badUserId = "BadValueId";
        private static readonly string _badUsername = "bad.user@email.com";
        private static readonly string _badPassword = "BadPassword";
        
        private IdentityUser _user;
        private IJwtTokenGenerator _tokenGenerator;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var tokenOptions = new TokenOptions(_issuer, _audience, _signingKey, _tokenExpiryInMinute);
            _tokenGenerator = new JwtTokenGenerator(tokenOptions);

            _user = new IdentityUser { Id = _userId, UserName = _username, PasswordHash = _password };

            _userManager = MockUserManager(_user).Object;
            _applicationDbContext = MockApplicationDbContext().Object;
        }

        [Test]
        public void CreateToken_CorrectCredentials_CreatesToken()
        {
            var tokenController = new TokenController(_applicationDbContext, _userManager, _tokenGenerator);
            var sut = tokenController.CreateToken(_username, _password, "password").Result;

            Assert.IsInstanceOf<ObjectResult>(sut);

            dynamic tokenObject = ((ObjectResult)sut).Value;
            Assert.IsInstanceOf<TokenModel>(tokenObject);
            Assert.AreEqual(_username, tokenObject.Username);
        }
        
        [Test]
        public void CreateToken_BadUsername_ReturnsBadRequest()
        {
            var tokenController = new TokenController(_applicationDbContext, _userManager, _tokenGenerator);
            var sut = tokenController.CreateToken(_badUsername, _password, "password").Result;

            Assert.IsInstanceOf<BadRequestResult>(sut);
        }
        
        [Test]
        public void CreateToken_BadPassword_ReturnsBadRequest()
        {
            var tokenController = new TokenController(_applicationDbContext, _userManager, _tokenGenerator);
            var sut = tokenController.CreateToken(_username, _badPassword, "password").Result;

            Assert.IsInstanceOf<BadRequestResult>(sut);
        }

        [Test]
        public void CreateToken_BadCredentials_ReturnsBadRequest()
        {
            var tokenController = new TokenController(_applicationDbContext, _userManager, _tokenGenerator);
            var sut = tokenController.CreateToken(_badUsername, _badPassword, "password").Result;

            Assert.IsInstanceOf<BadRequestResult>(sut);
        }

        [Test]
        public void RefreshToken_CorrectCredentials_CreatesToken()
        {
            var tokenController = new TokenController(_applicationDbContext, _userManager, _tokenGenerator);
            var sut = tokenController.RefreshToken(_userId).Result;

            Assert.IsInstanceOf<ObjectResult>(sut);

            dynamic tokenObject = ((ObjectResult)sut).Value;
            Assert.IsInstanceOf<TokenModel>(tokenObject);
            Assert.AreEqual(_username, tokenObject.Username);
        }
        
        [Test]
        public void RefreshToken_BadCredentials_CreatesToken()
        {
            var tokenController = new TokenController(_applicationDbContext, _userManager, _tokenGenerator);
            var sut = tokenController.RefreshToken(_badUserId).Result;

            Assert.IsInstanceOf<BadRequestResult>(sut);
        }
        
        [Test]
        public void Create2FaToken_CorrectCredentials_CreatesToken()
        {
            var tokenController = new TokenController(_applicationDbContext, _userManager, _tokenGenerator);
            var sut = tokenController.RefreshToken(_userId).Result;

            Assert.IsInstanceOf<ObjectResult>(sut);

            dynamic tokenObject = ((ObjectResult)sut).Value;
            Assert.IsInstanceOf<TokenModel>(tokenObject);
            Assert.AreEqual(_username, tokenObject.Username);
        }

        [Test]
        public void Create2FaToken_BadCredentials_CreatesToken()
        {
            var tokenController = new TokenController(_applicationDbContext, _userManager, _tokenGenerator);
            var sut = tokenController.Create2FaToken(_badUserId).Result;

            Assert.IsInstanceOf<BadRequestResult>(sut);
        }
        
        private static Mock<IApplicationUserManager<TUser>> MockUserManager<TUser>(TUser user) where TUser : class
        {
            var mgr = new Mock<IApplicationUserManager<TUser>>();
            // set up for success
            mgr.Setup(x => x.FindByEmailAsync(It.Is<string>(y => y == _username))).ReturnsAsync(user);
            mgr.Setup(x => x.FindByIdAsync(It.Is<string>(y => y == _userId))).ReturnsAsync(user);
            mgr.Setup(x => x.CheckPasswordAsync(It.IsAny<TUser>(), It.Is<string>(y => y == _password))).ReturnsAsync(true);
            // set up for fail
            mgr.Setup(x => x.FindByEmailAsync(It.Is<string>(y => y == _badUsername))).ReturnsAsync((TUser) null);
            mgr.Setup(x => x.FindByIdAsync(It.Is<string>(y => y == _badUserId))).ReturnsAsync((TUser) null);
            mgr.Setup(x => x.CheckPasswordAsync(It.IsAny<TUser>(), It.Is<string>(y => y == _badPassword))).ReturnsAsync(false);

            return mgr;
        }

        private static Mock<IApplicationDbContext> MockApplicationDbContext()
        {
            var context = new Mock<IApplicationDbContext>();
            context.Setup(x => x.GetRolesForUser(It.IsAny<IdentityUser>())).Returns(new List<string>());
            return context;
        }
    }
}