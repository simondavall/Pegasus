using NUnit.Framework;

namespace PegasusTests
{
    class TaskListControllerTests
    {
        [Test]
        public void TaskListController_IsValidSetting_ReturnsTrue()
        {
            var fromRequest = "123";

            var result = int.TryParse(fromRequest, out var id);
            Assert.IsTrue(result);
            Assert.AreEqual(id, 123);
        }

        [Test]
        public void TaskListController_InputStringIsNull_ReturnsFalse()
        {
            var result = int.TryParse(null, out _);
            Assert.IsFalse(result);
        }

        [Test]
        public void TaskListController_InputStringIsEmpty_ReturnsFalse()
        {
            string fromRequest = string.Empty;

            var result = int.TryParse(fromRequest, out _);
            Assert.IsFalse(result);
        }

        [Test]
        public void TaskListController_InputStringIsWhitespace_ReturnsFalse()
        {
            string fromRequest = "   ";

            var result = int.TryParse(fromRequest, out _);
            Assert.IsFalse(result);
        }
    }
}
