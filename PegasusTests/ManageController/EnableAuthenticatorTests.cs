using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Pegasus.Library.Api;
using Pegasus.Library.Models.Manage;

namespace PegasusTests.ManageController
{
    public class EnableAuthenticatorTests
    {
        private Mock<ILogger<Pegasus.Controllers.ManageController>> _logger; 
        private Mock<IManageEndpoint> _manageEndpoint;
        private ControllerContext _controllerContext;

        [SetUp]
        public void TestSetup()
        {
            _logger = new Mock<ILogger<Pegasus.Controllers.ManageController>>();
            var authenticatorKeyModel = new AuthenticatorKeyModel
            {
                AuthenticatorUri = "https://validAuthenticator",
                SharedKey = "asdf asdf asdf asdf"
            };
            _manageEndpoint = new Mock<IManageEndpoint>();
            _manageEndpoint.Setup(x => x.LoadSharedKeyAndQrCodeUriAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(authenticatorKeyModel));

            var identity = new Mock<IIdentity>()
            {
                Name = "simon.davall@gmail.com"
            };
            var user = new ClaimsPrincipal(identity.Object);
            var context = new DefaultHttpContext {User = user};
            _controllerContext = new ControllerContext
            {
                HttpContext = context,
            };
        }

        [Test]
        public async Task EnableAuthenticator_CallGetMethod_ReturnsCorrectType()
        {
            var sut = new Pegasus.Controllers.ManageController(_manageEndpoint.Object, _logger.Object)
            {
                ControllerContext = _controllerContext
            };

            var result = await sut.EnableAuthenticator();

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<EnableAuthenticatorModel>(((ViewResult) result).Model);
        }

        [Test]
        public async Task EnableAuthenticator_ModelStateNotValid_ReturnsCurrentView()
        {
            var sut = new Pegasus.Controllers.ManageController(_manageEndpoint.Object, _logger.Object)
            {
                ControllerContext = _controllerContext
            };
            sut.ModelState.AddModelError("TestError", "Something went wrong");

            var model = new EnableAuthenticatorModel
            {
                AuthenticatorUri = "https://originalbaseUri/",
                SharedKey = "original shared key"
            };
            var result = await sut.EnableAuthenticator(model);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<EnableAuthenticatorModel>(((ViewResult) result).Model);
            var returnedModel = (EnableAuthenticatorModel)((ViewResult) result).Model;
            Assert.AreEqual("https://validAuthenticator", returnedModel.AuthenticatorUri);
        }
    }
}
