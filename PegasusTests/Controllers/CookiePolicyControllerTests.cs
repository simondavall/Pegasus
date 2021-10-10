using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Services;
using Pegasus.Services.Models;

namespace PegasusTests.Controllers
{
    class CookiePolicyControllerTests
    {
        [Test]
        public void POST_SaveSelected_ReturnsPartialView()
        {
            var mockSettingsService = new Mock<ISettingsService>();
            mockSettingsService.Setup(x => x.SaveSettings());
            mockSettingsService.SetupGet(x => x.Settings).Returns(new SettingsModel());

            var sut = new CookiePolicyController(mockSettingsService.Object);
            var result = sut.SaveSelected(new SettingsModel());

            mockSettingsService.Verify(x => x.SaveSettings(), Times.Exactly(1));
            Assert.IsInstanceOf<PartialViewResult>(result);
        }
    }
}
