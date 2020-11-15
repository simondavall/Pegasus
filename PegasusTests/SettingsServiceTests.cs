using System;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;
using Pegasus.Services;

namespace PegasusTests
{
    class SettingsServiceTests
    {
        private IConfiguration _configuration;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        [SetUp]
        public void Setup()
        {
            // setup configuration from json string
            const string myJsonConfig = "{\"PegasusSettings\":{\"TaskFilterId\":125,\"CookieExpiryDays\":20,\"PageSize\":25}}";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(myJsonConfig));

            _configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpContextAccessor.Setup(x => x.HttpContext.Request.Cookies[It.IsAny<string>()])
                .Returns(
                    "{\"CookieExpiryDays\":180,\"PaginationEnabled\":true,\"ProjectId\":2,\"TaskFilterId\":17}");
        }

        [Test]
        public void SettingsService_SettingResidesInCookie_ReturnsCookieValue()
        {
            var sut = new SettingsService(_mockHttpContextAccessor.Object, _configuration);
            var result = sut.TaskFilterId;
            Assert.AreEqual(17, result);
        }

        [Test]
        public void SettingsService_SettingResidesInConfigButNotCookie_ReturnsConfigValue()
        {
            var sut = new SettingsService(_mockHttpContextAccessor.Object, _configuration);
            var result = sut.PageSize;
            Assert.AreEqual(25, result);
        }

        [Test]
        public void SettingsService_NullSettingName_ThrowsArgumentNullException()
        {
            var sut = new SettingsService(_mockHttpContextAccessor.Object, _configuration);
            Assert.Throws<ArgumentNullException>(() => sut.GetSetting<int>(null));
        }

        [Test]
        public void SettingsService_SettingDoesNotExist_ThrowsPropertyNotFoundException()
        {
            var sut = new SettingsService(_mockHttpContextAccessor.Object, _configuration);
            Assert.Throws<PropertyNotFoundException>(() => sut.GetSetting<int>("DoesNotExist"));
        }

        [Test]
        public void SettingsService_SettingWrongType_ThrowsInvalidTypeException()
        {
            var sut = new SettingsService(_mockHttpContextAccessor.Object, _configuration);
            Assert.Throws<PropertyTypeInvalidException>(() => sut.GetSetting<string>("TaskFilterId"));
        }

        [Test]
        public void SettingsService_SettingNotInCookieOrConfig_ReturnsDefaultValue()
        {
            const string myJsonConfig = "{\"PegasusSettings\":{\"TaskFilterId\":125,\"CookieExpiryDays\":20}}";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(myJsonConfig));
            _configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            var sut = new SettingsService(_mockHttpContextAccessor.Object, _configuration);
            var result = sut.PageSize;
            Assert.AreEqual(0, result);
        }
    }
}
