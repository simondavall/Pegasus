using System;
using NUnit.Framework;
using Pegasus.Extensions;

namespace PegasusTests.ExtensionTests
{
    class DateTimeExtensionTests
    {
        [Test]
        public void ToTaskDate_IsTimeValue_ForTodayDate()
        {
            var sut = DateTime.Now.Date.Date
                .AddHours(7)
                .AddMinutes(45);

            Assert.AreEqual("07:45", sut.ToTaskDate());
        }

        [Test]
        public void ToTaskDate_IsDateFormatValue_ForMoreThanADayAgo()
        {
            var sut = new DateTime(2017, 7, 15);

            Assert.AreEqual("Jul 15", sut.ToTaskDate());
        }

        [Test]
        public void TimeLapsed_For5mins()
        {
            var sut = DateTime.Now.AddMinutes(-5);

            Assert.AreEqual("5 mins ago", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For10mins()
        {
            var sut = DateTime.Now.AddMinutes(-10);

            Assert.AreEqual("10 mins ago", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For10hours()
        {
            var sut = DateTime.Now.AddHours(-10).AddMinutes(-10);

            Assert.AreEqual("10 hrs ago", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For1hours()
        {
            var sut = DateTime.Now.AddHours(-1).AddMinutes(-10);

            Assert.AreEqual("1 hr ago", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For3days()
        {
            var sut = DateTime.Now.AddDays(-3).AddHours(-10).AddMinutes(-10);

            Assert.AreEqual("3 days ago", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For28days_Returns28days()
        {
            var sut = DateTime.Now.AddMonths(-0).AddDays(-28).AddHours(-10).AddMinutes(-0);

            Assert.AreEqual("28 days ago", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For31days_Returns1month()
        {
            var sut = DateTime.Now.AddMonths(-0).AddDays(-31).AddHours(-10).AddMinutes(-0);

            Assert.AreEqual("1 mth ago", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For4months()
        {
            var sut = DateTime.Now.AddMonths(-4).AddDays(-3).AddHours(-10).AddMinutes(-10);

            Assert.AreEqual("4 mths ago", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For11months()
        {
            var sut = DateTime.Now.AddMonths(-11).AddDays(-3).AddHours(-10).AddMinutes(-10);

            Assert.AreEqual("11 mths ago", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For12months()
        {
            var sut = DateTime.Now.AddMonths(-12).AddDays(-0).AddHours(-0).AddMinutes(-0);

            Assert.AreEqual("1 yr ago", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_LessThanOneMinute_ReturnsJustNow()
        {
            var sut = DateTime.Now.AddSeconds(-55);

            Assert.AreEqual("just now", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_MoreThanOneMinute_DoesNotReturnJustNow()
        {
            var sut = DateTime.Now.AddSeconds(-65);

            Assert.AreNotEqual("just now", sut.LapsedTime());
        }

    }
}
