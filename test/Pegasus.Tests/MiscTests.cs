using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Pegasus.Tests
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class MiscTests
    {
        [Fact]
        public void PoorInputStringToTryParse_IntIsZero()
        {
            int projectId;
            string testStr = "1,2";

            int.TryParse(testStr, out projectId);

            Assert.Equal(0, projectId);
        }

        [Fact]
        public void NullInputStringToTryParse_IntIsZero()
        {
            int projectId;
            string testStr = null;

            int.TryParse(testStr, out projectId);

            Assert.Equal(0, projectId);
        }
    }
}
