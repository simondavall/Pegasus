using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Pegasus.Extensions;

namespace PegasusTests.ExtensionTests
{
    class ConfigurationExtensionTests
    {
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            // setup configuration from json string
            var myJsonConfig = "{\"Settings\": {\"IntSetting\": \"125\",\"StringSetting\":  \"Twenty\"}}";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(myJsonConfig));

            _configuration = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
        }

        [Test]
        public void FromConfig_RequestForInt_ReturnsInt()
        {
            var result = _configuration.FromConfig("Settings:IntSetting", 123);

            Assert.IsInstanceOf<int>(result);
            Assert.AreEqual(125, result);
        }

        [Test]
        public void FromConfig_RequestForString_ReturnsString()
        {
            var result = _configuration.FromConfig("Settings:StringSetting", "Ten");

            Assert.IsInstanceOf<string>(result);
            Assert.AreEqual("Twenty", result);
        }

        [Test]
        public void FromConfig_RequestForNonExistentIntSetting_ReturnsDefault()
        {
            var result = _configuration.FromConfig("DoesNotExist", 123);

            Assert.IsInstanceOf<int>(result);
            Assert.AreEqual(123, result);
        }

        [Test]
        public void FromConfig_RequestForNonExistentStringSetting_ReturnsDefault()
        {
            var result = _configuration.FromConfig("DoesNotExist", "Ten");

            Assert.IsInstanceOf<string>(result);
            Assert.AreEqual("Ten", result);
        }

        [Test]
        public void FromConfig_RequestForNonExistentSettingWithNull_ReturnsNull()
        {
            var result = _configuration.FromConfig("DoesNotExist", null);

            Assert.IsNull(result);
        }
    }
}
