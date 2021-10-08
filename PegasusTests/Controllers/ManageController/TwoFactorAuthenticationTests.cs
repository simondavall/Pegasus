using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Library.Api;
using Pegasus.Library.Models.Manage;
using Pegasus.Services;

namespace PegasusTests.Controllers.ManageController
{
    class TwoFactorAuthenticationTests : ManageControllerBase
    {
        [Test]
        public async Task TwoFactorAuthentication_CallsApi_ReturnsViewResult()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>())).ReturnsAsync(new TwoFactorAuthenticationModel());
            var manageEndpoint = new ManageEndpoint(_mockApiHelper.Object);

            var signInManager = new SignInManager(_mockHttpContextWrapper.Object, _mockAccountsEndpoint.Object, _mockApiHelper.Object,_mockTokenAccessor.Object, _mockAuthenticationEndpoint.Object);

            var sut = new Pegasus.Controllers.ManageController(manageEndpoint, signInManager, _logger.Object)
            {
                ControllerContext = _controllerContext
            };

            var result = await sut.TwoFactorAuthentication(new TwoFactorAuthenticationModel());

            _mockHttpContextWrapper.Verify(x => x.SignOutAsync(It.IsAny<string>()), Times.Exactly(1));
            _mockApiHelper.Verify(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
        }
    }
}
