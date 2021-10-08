using NUnit.Framework;
using Pegasus.Extensions;
using Pegasus.Services;

namespace PegasusTests.ExtensionTests
{
    class SettingsServiceExtensionTests
    {
        [Test]
        public void GetDisplayName_WithPropertyName_ReturnsDisplayName()
        {
            var service = new SettingsService();
            var sut = service.GetDisplayName(nameof(service.Settings.PaginationEnabled));

            Assert.AreEqual("Pagination Enabled", sut);
        }
    }
}
