using Moq;
using NUnit.Framework;
using Pegasus.Domain;
using Pegasus.Library.JwtAuthentication.Constants;
using Pegasus.Library.Services.Http;
using Pegasus.Services;
using Pegasus.Services.Models;

namespace PegasusTests.ServiceTests
{
    class AnalyticsServiceTests
    {
        private Mock<IHttpContextWrapper> _mockHttpContextWrapper;
        private Mock<ISettingsService> _mockSettingsService;
        private Mock<ICookies> _mockCookies;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _mockHttpContextWrapper = new Mock<IHttpContextWrapper>();
            _mockSettingsService = new Mock<ISettingsService>();
            _mockCookies = new Mock<ICookies>();
        }

        [Test]
        public void SaveAnalyticsData_AnalyticsCookieAllowed_SavesDataToCookie()
        {
            var settingsModel = new SettingsModel { AnalyticsCookieEnabled = true };
            _mockSettingsService.Setup(x => x.Settings).Returns(settingsModel);
            _mockCookies.Setup(x => x.WriteCookie(CookieConstants.Analytics, It.IsAny<string>()));

            var sut = new TestableAnalyticsService(_mockHttpContextWrapper.Object, _mockSettingsService.Object, _mockCookies.Object);
            sut.SaveAnalyticsData("Some data");

            _mockCookies.Verify(
                x => x.WriteCookie(CookieConstants.Analytics, It.IsAny<string>()),
                Times.Exactly(1));
        }

        [Test]
        public void SaveAnalyticsData_AnalyticsCookieNotAllowed_DeleteCookie()
        {
            var settingsModel = new SettingsModel { AnalyticsCookieEnabled = false };
            _mockSettingsService.Setup(x => x.Settings).Returns(settingsModel);

            _mockCookies.Setup(x => x.DeleteCookie(CookieConstants.Analytics));

            var sut = new TestableAnalyticsService(_mockHttpContextWrapper.Object, _mockSettingsService.Object, _mockCookies.Object);
            sut.SaveAnalyticsData("Some data");

            _mockCookies.Verify(
                x => x.DeleteCookie(CookieConstants.Analytics),
                Times.Exactly(1));
        }

        private class TestableAnalyticsService : AnalyticsService
        {
            public TestableAnalyticsService(IHttpContextWrapper httpContextWrapper, ISettingsService settingsService, ICookies cookies = null) : base(httpContextWrapper, settingsService)
            {
                if(!(cookies is null))
                    Cookies = cookies;
            }
        }
    }
}
