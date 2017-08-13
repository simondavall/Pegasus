using System;
using System.Diagnostics.CodeAnalysis;
using Pegasus.Extensions;
using Xunit;

namespace Pegasus.Tests
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Tests
    {
        [Fact]
        public void IsToDay_IsValidForTodaysDate()
        {
            var sut = DateTime.Now;
            Assert.Equal(DateTime.Today, sut.Date);
        }

        [Fact]
        public void ToTaskDate_IsTimeValue_ForTodaysDate()
        {
            var sut = DateTime.Now.Date.Date
                .AddHours(7)
                .AddMinutes(45)
                .ToTaskDate();
            Assert.Equal("07:45", sut);
        }

        [Fact]
        public void ToTaskDate_IsDateValue_ForYesterdaysDate()
        {
            var sut = new DateTime(2017,7,15).ToTaskDate();

            Assert.Equal("Jul 15", sut);
        }
    }
}
