using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using PegasusApi.Library.Exceptions;
using PegasusApi.Models;
using PegasusApi.Services;

namespace PegasusApi.Tests.Services
{
    public class EmailSenderTests
    {
        private Mock<IOptions<EmailSenderOptions>> _mockOptionsAccessor;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<ILogger<EmailSender>> _mockLogger;

        [SetUp]
        public void EachTestSetup()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockOptionsAccessor = new Mock<IOptions<EmailSenderOptions>>();
            _mockLogger = new Mock<ILogger<EmailSender>>();
        }

        [Test]
        public async Task Execute_NoApiKeyFound_ThrowsEmailException()
        {
            var sut = new EmailSender(_mockOptionsAccessor.Object, _mockConfiguration.Object, _mockLogger.Object);
            
            await sut.Invoking(x => x.Execute(null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Should().ThrowAsync<EmailException>();
            
            await sut.Invoking(x => x.Execute(null, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Should().ThrowExactlyAsync<EmailApiKeyNotFoundException>();
        }
    }
}