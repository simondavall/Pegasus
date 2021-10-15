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
            MockApiHelper.Setup(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>())).ReturnsAsync(new TwoFactorAuthenticationModel());

            var sut = CreateManageController();

            var result = await sut.TwoFactorAuthentication(null);

            MockApiHelper.Verify(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task TwoFactorAuthentication_ModelUserIdIsNull_CallsModelApi()
        {
            MockApiHelper.Setup(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>())).ReturnsAsync(new TwoFactorAuthenticationModel());

            var sut = CreateManageController();
            var result = await sut.TwoFactorAuthentication(new TwoFactorAuthenticationModel {UserId = null});

            MockApiHelper.Verify(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task TwoFactorAuthentication_ModelUserIdNotNull_DoesNotCallsModelApi()
        {
            MockApiHelper.Setup(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>())).ReturnsAsync(new TwoFactorAuthenticationModel());

            var sut = CreateManageController();
            var result = await sut.TwoFactorAuthentication(new TwoFactorAuthenticationModel {UserId = UserId});

            MockApiHelper.Verify(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>()), Times.Never);
            Assert.IsInstanceOf<ViewResult>(result);
        }


        [Test]
        public async Task ForgetThisBrowser_CallsApi_ReturnsRedirectToActionWithStatusAndUserId()
        {
            MockApiHelper.Setup(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>())).ReturnsAsync(new TwoFactorAuthenticationModel{UserId = UserId});

            var sut = CreateManageController();
            var result = await sut.ForgetThisBrowser();

            MockHttpContextWrapper.Verify(x => x.SignOutAsync(It.IsAny<string>()), Times.Exactly(1));
            MockApiHelper.Verify(x => x.GetFromUri<TwoFactorAuthenticationModel>(It.IsAny<string>()), Times.Exactly(1));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual(Resources.ControllerStrings.ManageController.CurrentBrowserForgotten, ((RedirectToActionResult)result).RouteValues["StatusMessage"]);
            Assert.NotNull(((RedirectToActionResult)result).RouteValues["UserId"], "UserId check failed.");
        }
    }
}
