using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private ILogger<TokenController> _logger;
        private TokenController _tokenController;

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
            _logger = new Mock<ILogger<TokenController>>().Object;
            _tokenController = new TokenController(_applicationDbContext, UserManager, _tokenGenerator, _logger);
        }

        [Test]
        public void CreateToken_CorrectCredentials_CreatesToken()
        {
            var sut = _tokenController.CreateToken(Username, Password, "password").Result;

            Assert.IsInstanceOf<ObjectResult>(sut);

            dynamic tokenObject = ((ObjectResult)sut).Value;
            Assert.IsInstanceOf<TokenModel>(tokenObject);
            Assert.AreEqual(Username, tokenObject.Username);
        }
        
        [Test]
        public void CreateToken_BadUsername_ReturnsBadRequest()
        {
            var sut = _tokenController.CreateToken(BadUsername, Password, "password").Result;

            Assert.IsInstanceOf<NotFoundResult>(sut);
        }
        
        [Test]
        public void CreateToken_BadPassword_ReturnsBadRequest()
        {
            var sut = _tokenController.CreateToken(Username, BadPassword, "password").Result;

            Assert.IsInstanceOf<BadRequestResult>(sut);
        }

        [Test]
        public void CreateToken_BadCredentials_ReturnsBadRequest()
        {
            var sut = _tokenController.CreateToken(BadUsername, BadPassword, "password").Result;

            Assert.IsInstanceOf<NotFoundResult>(sut);
        }

        [Test]
        public void RefreshToken_CorrectCredentials_CreatesToken()
        {
            var sut = _tokenController.RefreshToken(UserId).Result;

            Assert.IsInstanceOf<ObjectResult>(sut);

            dynamic tokenObject = ((ObjectResult)sut).Value;
            Assert.IsInstanceOf<TokenModel>(tokenObject);
            Assert.AreEqual(Username, tokenObject.Username);
        }
        
        [Test]
        public void RefreshToken_BadCredentials_CreatesToken()
        {
            var sut = _tokenController.RefreshToken(BadUserId).Result;

            Assert.IsInstanceOf<NotFoundResult>(sut);
        }
        
        [Test]
        public void Create2FaToken_CorrectCredentials_CreatesToken()
        {
            var sut = _tokenController.RefreshToken(UserId).Result;

            Assert.IsInstanceOf<ObjectResult>(sut);

            dynamic tokenObject = ((ObjectResult)sut).Value;
            Assert.IsInstanceOf<TokenModel>(tokenObject);
            Assert.AreEqual(Username, tokenObject.Username);
        }

        [Test]
        public void Create2FaToken_BadCredentials_CreatesToken()
        {
            var sut = _tokenController.Create2FaToken(BadUserId).Result;

            Assert.IsInstanceOf<NotFoundResult>(sut);
        }
        
        private static Mock<IApplicationDbContext> MockApplicationDbContext()
        {
            var context = new Mock<IApplicationDbContext>();
            context.Setup(x => x.GetRolesForUser(It.IsAny<IdentityUser>())).Returns(new List<string>());
            return context;
        }
    }
}