using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Library.Models.Manage;
using Pegasus.Library.Services.Resources;

namespace PegasusTests.Controllers.ManageControllerTests
{
    class TwoFactorAuthenticationTests : ManageControllerTestsBase
    {
        [Test]
        public async Task TwoFactorAuthentication_ModelIsNull_CallsModelApi()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>())).ReturnsAsync(new TwoFactorAuthenticationModel());

            var sut = CreateManageController();

            var result = await sut.TwoFactorAuthentication(null);

            _mockApiHelper.Verify(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task TwoFactorAuthentication_ModelUserIdIsNull_CallsModelApi()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>())).ReturnsAsync(new TwoFactorAuthenticationModel());

            var sut = CreateManageController();
            var result = await sut.TwoFactorAuthentication(new TwoFactorAuthenticationModel {UserId = null});

            _mockApiHelper.Verify(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task TwoFactorAuthentication_ModelUserIdNotNull_DoesNotCallsModelApi()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>())).ReturnsAsync(new TwoFactorAuthenticationModel());

            var sut = CreateManageController();
            var result = await sut.TwoFactorAuthentication(new TwoFactorAuthenticationModel {UserId = UserId});

            _mockApiHelper.Verify(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>()), Times.Never);
            Assert.IsInstanceOf<ViewResult>(result);
        }


        [Test]
        public async Task ForgetThisBrowser_CallsApi_ReturnsRedirectToActionWithStatusAndUserId()
        {
            _mockApiHelper.Setup(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>())).ReturnsAsync(new TwoFactorAuthenticationModel{UserId = UserId});

            var sut = CreateManageController();
            var result = await sut.ForgetThisBrowser();

            _mockHttpContextWrapper.Verify(x => x.SignOutAsync(It.IsAny<string>()), Times.Exactly(1));
            _mockApiHelper.Verify(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(Resources.ControllerStrings.ManageController.CurrentBrowserForgotten, ((RedirectToActionResult)result).RouteValues["StatusMessage"]);
            Assert.NotNull(((RedirectToActionResult)result).RouteValues["UserId"], "UserId check failed.");
        }
    }
}
