using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pegasus.Entities.Enumerations;
using Pegasus.Entities.Sorters.ProjectTask;
using Pegasus.Extensions;
using Pegasus.Library.Models;
using Pegasus.Models.Settings;
using Pegasus.Models.TaskList;

namespace PegasusTests.ProjectTask_Tests
{
    class ProjectTaskTests
    {
        private readonly ISettingsModel _settingsModel = new SettingsModel();

        [SetUp]
        public void TestSetup()
        {
            _settingsModel.PaginationEnabled = true;
        }

        [Test]
        public void ProjectTask_SortsByModifiedDate_ReturnsMostRecentModifiedFirst()
        {
            var taskList = new List<TaskModel>
            {
                new TaskModel { Modified = new DateTime(2020, 1, 1) },
                new TaskModel { Modified = new DateTime(2020, 2, 1) }
            };

            var sut = new IndexViewModel(taskList, (int)TaskFilters.All, _settingsModel);
            var sorter = new ModifiedDescSorter();

            Assert.IsNotEmpty(sut.ProjectTasks);
            Assert.AreEqual(sut.ProjectTasks.Sorted(sorter).ToArray()[0].Modified, new DateTime(2020, 2, 1));
        }

        [Test]
        public void ProjectTask_SortsByPriorityDateDesc_ReturnsHighestPriorityFirst()
        {
            var taskList = new List<TaskModel>
            {
                new TaskModel { TaskPriorityId = (int)TaskPriorityEnum.Low },
                new TaskModel { TaskPriorityId = (int)TaskPriorityEnum.Critical }
            };
        
            var sut = new IndexViewModel(taskList, (int)TaskFilters.All, _settingsModel);
            var sorter = new PriorityDescSorter();

            Assert.IsNotEmpty(sut.ProjectTasks);
            Assert.AreEqual(sut.ProjectTasks.Sorted(sorter).ToArray()[0].TaskPriorityId, (int)TaskPriorityEnum.Critical);
        }

        [Test]
        public void ProjectTask_SortsByPriorityDateAsc_ReturnsLowestPriorityFirst()
        {
            var taskList = new List<TaskModel>
            {
                new TaskModel { TaskPriorityId = (int)TaskPriorityEnum.High },
                new TaskModel { TaskPriorityId = (int)TaskPriorityEnum.Low },
                new TaskModel { TaskPriorityId = (int)TaskPriorityEnum.Critical }
            };

            var sut = new IndexViewModel(taskList, (int)TaskFilters.All, _settingsModel);
            var sorter = new PriorityAscSorter();

            Assert.IsNotEmpty(sut.ProjectTasks);
            Assert.AreEqual(sut.ProjectTasks.Sorted(sorter).ToArray()[0].TaskPriorityId, (int)TaskPriorityEnum.Low);
        }

        [Test]
        public void ProjectTask_SortsByTaskRefDesc_ReturnsHighestTaskRefFirst()
        {
            var taskList = new List<TaskModel>
            {
                new TaskModel { TaskRef = "1" },
                new TaskModel { TaskRef = "2" },
                new TaskModel { TaskRef = "3" }
            };
        
            var sut = new IndexViewModel(taskList, (int)TaskFilters.All, _settingsModel);
            var sorter = new TaskRefDescSorter();
        
            Assert.IsNotEmpty(sut.ProjectTasks);
            Assert.AreEqual(sut.ProjectTasks.Sorted(sorter).ToArray()[0].TaskRef, "3");
        }
    }
}
