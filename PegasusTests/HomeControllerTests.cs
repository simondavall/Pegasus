using NUnit.Framework;

namespace PegasusTests
{
    class HomeControllerTests
    {
        [Test]
        public void HomeController_IsValidSetting_ReturnsTrue()
        {
            var fromRequest = "123";

            var result = int.TryParse(fromRequest, out var id);
            Assert.IsTrue(result);
            Assert.AreEqual(id, 123);
        }

        [Test]
        public void HomeController_InputStringIsNull_ReturnsFalse()
        {
            var result = int.TryParse(null, out _);
            Assert.IsFalse(result);
        }

        [Test]
        public void HomeController_InputStringIsEmpty_ReturnsFalse()
        {
            string fromRequest = string.Empty;

            var result = int.TryParse(fromRequest, out _);
            Assert.IsFalse(result);
        }

        [Test]
        public void HomeController_InputStringIsWhitespace_ReturnsFalse()
        {
            string fromRequest = "   ";

            var result = int.TryParse(fromRequest, out _);
            Assert.IsFalse(result);
        }
    }
}
