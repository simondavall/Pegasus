using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pegasus.Extensions;
using Pegasus.Library.Models;

namespace PegasusTests.ProjectTask_Tests
{
    public class PaginationTests
    {
        private IEnumerable<TaskModel> _taskList;

        [SetUp]
        public void TestSetup()
        {
            _taskList = new List<TaskModel>
            {
                new TaskModel { Modified = new DateTime(2010, 1, 1) },
                new TaskModel { Modified = new DateTime(2011, 1, 1) },
                new TaskModel { Modified = new DateTime(2012, 1, 1) },
                new TaskModel { Modified = new DateTime(2013, 1, 1) },
                new TaskModel { Modified = new DateTime(2014, 1, 1) },
                new TaskModel { Modified = new DateTime(2015, 1, 1) },
                new TaskModel { Modified = new DateTime(2016, 1, 1) },
                new TaskModel { Modified = new DateTime(2017, 1, 1) },
                new TaskModel { Modified = new DateTime(2018, 1, 1) },
                new TaskModel { Modified = new DateTime(2019, 1, 1) },
                new TaskModel { Modified = new DateTime(2020, 1, 1) },
                new TaskModel { Modified = new DateTime(2021, 1, 1) },

                new TaskModel { Modified = new DateTime(2022, 1, 1) },
                new TaskModel { Modified = new DateTime(2023, 1, 1) },
                new TaskModel { Modified = new DateTime(2024, 1, 1) },
                new TaskModel { Modified = new DateTime(2025, 1, 1) },
                new TaskModel { Modified = new DateTime(2026, 1, 1) },
                new TaskModel { Modified = new DateTime(2027, 1, 1) },
                new TaskModel { Modified = new DateTime(2028, 1, 1) },
                new TaskModel { Modified = new DateTime(2029, 1, 1) },
                new TaskModel { Modified = new DateTime(2030, 1, 1) }
            };
        }

        [Test]
        public void Pagination_PageOne_ReturnsPageOneItems()
        {
            var page = 1;
            var pageSize = 12;

            var sut = _taskList.Paginated(page, pageSize).ToList();

            Assert.AreEqual(pageSize, sut.Count);
            Assert.AreEqual(new DateTime(2010, 1, 1), sut[0].Modified);
        }

        [Test]
        public void Pagination_PageTwo_ReturnsThirteenthItem()
        {
            var page = 2;
            var pageSize = 12;
            var sut = _taskList.Paginated(page, pageSize).ToList();

            Assert.AreEqual(9, sut.Count);
            Assert.AreEqual(new DateTime(2022, 1, 1), sut[0].Modified);
        }
    }
}
