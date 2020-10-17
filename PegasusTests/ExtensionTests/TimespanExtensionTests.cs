﻿using System;
using NUnit.Framework;
using Pegasus.Extensions;

namespace PegasusTests.ExtensionTests
{
    class TimespanExtensionTests
    {

        [Test]
        public void TimeLapsed_For5mins()
        {
            var sut = DateTime.Now.AddMinutes(-5);

            Assert.AreEqual("5 mins", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For10mins()
        {
            var sut = DateTime.Now.AddMinutes(-10);

            Assert.AreEqual("10 mins", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For10hours()
        {
            var sut = DateTime.Now.AddHours(-10).AddMinutes(-10);

            Assert.AreEqual("10 hrs", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For1hours()
        {
            var sut = DateTime.Now.AddHours(-1).AddMinutes(-10);

            Assert.AreEqual("1 hr", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For3days()
        {
            var sut = DateTime.Now.AddDays(-3).AddHours(-10).AddMinutes(-10);

            Assert.AreEqual("3 days", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For29days_Returns29days()
        {
            var sut = DateTime.Now.AddMonths(-0).AddDays(-30).AddHours(-10).AddMinutes(-0);

            Assert.AreEqual("1 mth", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For30days_Returns1month()
        {
            var sut = DateTime.Now.AddMonths(-0).AddDays(-30).AddHours(-10).AddMinutes(-0);

            Assert.AreEqual("1 mth", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For4months()
        {
            var sut = DateTime.Now.AddMonths(-4).AddDays(-3).AddHours(-10).AddMinutes(-10);

            Assert.AreEqual("4 mths", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For11months()
        {
            var sut = DateTime.Now.AddMonths(-11).AddDays(-3).AddHours(-10).AddMinutes(-10);

            Assert.AreEqual("11 mths", sut.LapsedTime());
        }

        [Test]
        public void TimeLapsed_For12months()
        {
            var sut = DateTime.Now.AddMonths(-12).AddDays(-0).AddHours(-0).AddMinutes(-0);

            Assert.AreEqual("1 yr", sut.LapsedTime());
        }

    }
}