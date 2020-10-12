using NUnit.Framework;

namespace PegasusTests
{
    public class MiscTests
    {
        [Test]
        public void PoorInputStringToTryParse_IntIsZero()
        {
            const string testStr = "1,2";

            int.TryParse(testStr, out var projectId);

            Assert.AreEqual(0, projectId);
        }

        [Test]
        public void NullInputStringToTryParse_IntIsZero()
        {
            const string testStr = null;

            int.TryParse(testStr, out var projectId);

            Assert.AreEqual(0, projectId);
        }
    }
}
