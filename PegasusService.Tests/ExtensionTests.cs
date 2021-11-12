using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using PegasusService.Extensions;

namespace PegasusService.Tests
{
    public class Tests
    {
        private readonly Mock<IConfiguration> _mockConfig;

        public Tests()
        {
            _mockConfig = new Mock<IConfiguration>();
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _mockConfig.Setup(x => x["SomeKnownKey"]).Returns("123");
            _mockConfig.Setup(x => x["UnknownKey"]).Returns("");
        }

        [Test]
        public void FromConfig_PassKnownKey_ReturnsCorrectValue()
        {
            var sut = _mockConfig.Object;
            var result = sut.FromConfig("SomeKnownKey", -1);

            result.Should().Be(123);
        }

        [Test]
        public void FromConfig_PassUnKnownKey_ReturnsDefaultValue()
        {
            var sut = _mockConfig.Object;
            var result = sut.FromConfig("UnknownKey", -1);

            result.Should().Be(-1);
        }
    }
}