using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Pegasus.Domain;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace PegasusTests
{
    class SettingsTests
    {
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            // setup configuration from json string
            var myJsonConfig = "{\"DefaultSettings\": {\"taskFilterId\": \"125\",\"cookieExpiryDays\":  20}}";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(myJsonConfig));

            _configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
        }

        [Test]
        public void Settings_TestInMemoryConfig_ReturnsDefaultValue()
        {
            var sut = new Settings(_configuration);
            var result = sut.GetSetting("taskFilterId", 30);
            Assert.AreEqual(result, 125);
        }

        [Test]
        public void Settings_NonExistentSetting_ReturnsDefaultValue()
        {
            var sut = new Settings(_configuration);
            var result = sut.GetSetting("DoesNotExist", 30);
            Assert.AreEqual(result, 30);
        }

        [Test]
        public void Settings_RequestWithSetting_ReturnsRequestSetting()
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.QueryString = new QueryString("?taskFilterId=789");

            var sut = new Settings(_configuration);
            var result = sut.GetSetting(request, "taskFilterId", 35);
            Assert.AreEqual(result, 789);
        }

        [Test]
        public void Settings_RequestWithNoSetting_ReturnsConfigSetting()
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            //request.QueryString = new QueryString("?taskFilterId=789");

            var sut = new Settings(_configuration);
            var result = sut.GetSetting(request, "taskFilterId", 35);
            Assert.AreEqual(result, 125);
        }

        [Test]
        public void Settings_RequestWithNoSettingAndNoConfig_ReturnsDefaultValue()
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            //request.QueryString = new QueryString("?taskFilterId=789");

            var sut = new Settings(_configuration);
            var result = sut.GetSetting(request, "doesnotexist", 35);
            Assert.AreEqual(result, 35);
        }
    }
}
