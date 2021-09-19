using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PegasusApi.Controllers;
using PegasusApi.Data;
using PegasusApi.Library.JwtAuthentication;
using PegasusApi.Models;
using TokenOptions = PegasusApi.Library.JwtAuthentication.Models.TokenOptions;

namespace PegasusApi.Tests
{
    public class TokenControllerTests : BaseControllerTest
    {
        private IApplicationDbContext _applicationDbContext;
        private IJwtTokenGenerator _tokenGenerator;

        private readonly string _issuer = "https://fakelocalhost:5001";
        private readonly string _audience = "https://fakelocalhost:5002";
        private readonly string _signingKey = "MySecretKeyIsSecretSoDoNotTell";
        private readonly string _tokenExpiryInMinute = "720";

        [OneTimeSetUp]
        public override void OneTimeSetup()
        {
            base.OneTimeSetup();

            var tokenOptions = new TokenOptions(_issuer, _audience, _signingKey, _tokenExpiryInMinute);
            _tokenGenerator = new JwtTokenGenerator(tokenOptions);

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
        
        private static Mock<IApplicationDbContext> MockApplicationDbContext()
        {
            var context = new Mock<IApplicationDbContext>();
            context.Setup(x => x.GetRolesForUser(It.IsAny<IdentityUser>())).Returns(new List<string>());
            return context;
        }
    }
}