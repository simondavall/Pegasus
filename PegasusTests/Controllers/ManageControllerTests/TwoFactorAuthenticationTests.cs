using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Library.Models.Manage;
using ManageControllerStrings = Pegasus.Library.Services.Resources.Resources.ControllerStrings.ManageController;


namespace PegasusTests.Controllers.ManageControllerTests
{
    internal class TwoFactorAuthenticationTests : ManageControllerTestsBase
    {
        [Test]
        public async Task TwoFactorAuthentication_ModelIsNull_CallsModelApi()
        {
            MockManageEndpoint.Setup(x => x.TwoFactorAuthentication(It.IsAny<string>()))
                .ReturnsAsync(new TwoFactorAuthenticationModel());

            var sut = CreateManageController();
            var result = await sut.TwoFactorAuthentication(null);

            result.Should().BeOfType<ViewResult>();
        }

        [Test]
        public async Task TwoFactorAuthentication_ModelUserIdIsNull_CallsModelApi()
        {
            MockManageEndpoint.Setup(x => x.TwoFactorAuthentication(It.IsAny<string>()))
                .ReturnsAsync(new TwoFactorAuthenticationModel());

            var sut = CreateManageController();
            var result = await sut.TwoFactorAuthentication(new TwoFactorAuthenticationModel { UserId = null });

            result.Should().BeOfType<ViewResult>();
        }

        [Test]
        public async Task TwoFactorAuthentication_ModelUserIdNotNull_DoesNotCallsModelApi()
        {
            MockManageEndpoint.Setup(x => x.TwoFactorAuthentication(It.IsAny<string>()))
                .ReturnsAsync(new TwoFactorAuthenticationModel());

            var sut = CreateManageController();
            var result = await sut.TwoFactorAuthentication(new TwoFactorAuthenticationModel { UserId = UserId });

            result.Should().BeOfType<ViewResult>();
        }


        [Test]
        public async Task ForgetThisBrowser_CallsApi_ReturnsRedirectToActionWithStatusAndUserId()
        {
            MockManageEndpoint.Setup(x => x.TwoFactorAuthentication(It.IsAny<string>()))
                .ReturnsAsync(new TwoFactorAuthenticationModel { UserId = UserId });

            var sut = CreateManageController();
            var result = await sut.ForgetThisBrowser();

            result.Should().BeOfType<RedirectToActionResult>();
            var routeValues = ((RedirectToActionResult)result).RouteValues;
            routeValues["StatusMessage"].Should().Be(ManageControllerStrings.CurrentBrowserForgotten);
            routeValues["UserId"].Should().NotBeNull();
        }
    }
}