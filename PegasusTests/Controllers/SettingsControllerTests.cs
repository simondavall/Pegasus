using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Services;
using Pegasus.Services.Models;

namespace PegasusTests.Controllers
{
    class SettingsControllerTests
    {
        [Test]
        public void Index_ReturnsViewResult()
        {
            var sut = new SettingsController(new SettingsService());
            var result = sut.Index();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void SaveSettings_X_Y()
        {
            var returnUrl = "http://returnUrl";
            var mockSettingsService = new Mock<ISettingsService>();
            mockSettingsService.Setup(x => x.SaveSettings());
            mockSettingsService.SetupGet(x => x.Settings).Returns(new SettingsModel());

            var sut = new SettingsController(mockSettingsService.Object);
            var result = sut.SaveSettings(new SettingsModel(), returnUrl);

            Assert.IsInstanceOf<RedirectResult>(result);
            Assert.AreEqual(returnUrl, ((RedirectResult)result).Url);
        }
    }
}
