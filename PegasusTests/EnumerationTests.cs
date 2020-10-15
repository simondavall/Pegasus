using System;
using NUnit.Framework;
using Pegasus.Entities.Enumerations;

namespace PegasusTests
{
    class EnumerationTests
    {
        [Test]
        public void EnumerationHelper_GetDisplayNameForEnumeration()
        {
            var result = EnumHelper<TaskFilters>.GetDisplayValue(TaskFilters.All);
            Assert.AreEqual("All Tasks", result);
        }

        [Test]
        public void EnumerationHelper_ParseWithGoodValue_ReturnsEnumValue()
        {
            var result = EnumHelper<TaskFilters>.Parse("All");
            Assert.AreEqual(TaskFilters.All, result);
        }

        [Test]
        public void EnumerationHelper_ParseWithBadValue_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => EnumHelper<TaskFilters>.Parse("DoesNotExist"));
        }

        [Test]
        public void EnumerationHelper_ParseWithNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => EnumHelper<TaskFilters>.Parse(null));
        }

        [Test]
        public void EnumerationHelper_GetNamesWithNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => EnumHelper<TaskFilters>.GetNames(null));
        }

        [Test]
        public void EnumerationHelper_GetNamesWithGoodValue_Returns()
        {
            const TaskFilters enumValue = TaskFilters.Open;
            var result = EnumHelper<TaskFilters>.GetNames(enumValue);
            Assert.AreEqual(Enum.GetNames(typeof(TaskFilters)).Length, result.Count);
            Assert.AreEqual(Enum.GetNames(typeof(TaskFilters))[1], result[1]);
        }

        [Test]
        public void EnumerationHelper_GetDisplayNamesWithGoodValue_Returns()
        {
            const TaskFilters enumValue = TaskFilters.Open;
            var result = EnumHelper<TaskFilters>.GetDisplayValues(enumValue);
            Assert.AreEqual(Enum.GetNames(typeof(TaskFilters)).Length, result.Count);
            Assert.AreEqual("Open Tasks", result[1]);
        }

        [Test]
        public void EnumerationHelper_GetDisplayNamesWithNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => EnumHelper<TaskFilters>.GetDisplayValues(null));
        }

    }
}
