using System;
using NUnit.Framework;
using Pegasus.Extensions;

namespace PegasusTests.ExtensionTests
{
    class TimespanExtensionTests
    {
        private DateTime _date1;

        [SetUp]
        public void TestSetup()
        {
            _date1 = new DateTime(2020, 1, 1);
        }

        [Test]
        public void ApproxMonths_OneDateTimeMonth_ReturnsOneMonth()
        {
            var date2 = _date1.AddMonths(1);
            var sut = date2 - _date1;

            Assert.AreEqual(1, sut.ApproxMonths());
        }

        [Test]
        public void ApproxMonths_TwoDateTimeMonth_ReturnsTwoMonth()
        {
            var date2 = _date1.AddMonths(2);
            var sut = date2 - _date1;

            Assert.AreEqual(2, sut.ApproxMonths());
        }

        [Test]
        public void ApproxMonths_OneDateTimeYear_ReturnsTwelveMonth()
        {
            var date2 = _date1.AddYears(1);
            var sut = date2 - _date1;

            Assert.AreEqual(12, sut.ApproxMonths());
        }

        [Test]
        public void ApproxYears_OneDateTimeYear_ReturnsOneYear()
        {
            var date2 = _date1.AddYears(1);
            var sut = date2 - _date1;

            Assert.AreEqual(1, sut.ApproxYears());
        }

        [Test]
        public void ApproxYears_TenDateTimeYears_ReturnsTenYears()
        {
            var date2 = _date1.AddYears(10);
            var sut = date2 - _date1;

            Assert.AreEqual(10, sut.ApproxYears());
        }

    }
}
