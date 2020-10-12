using System;
using NUnit.Framework;
using Pegasus.Extensions;

namespace PegasusTests
{
    public class ExtensionTests
    {
        [Test]
        public void IsToDay_IsValidForTodayDate()
        {
            var sut = DateTime.Now;
            Assert.AreEqual(DateTime.Today, sut.Date);
        }

        [Test]
        public void ToTaskDate_IsTimeValue_ForTodayDate()
        {
            var sut = DateTime.Now.Date.Date
                .AddHours(7)
                .AddMinutes(45)
                .ToTaskDate();
            Assert.AreEqual("07:45", sut);
        }

        [Test]
        public void ToTaskDate_IsDateValue_ForYesterdaysDate()
        {
            var sut = new DateTime(2017,7,15).ToTaskDate();

            Assert.AreEqual("Jul 15", sut);
        }
    }
}
