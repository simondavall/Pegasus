using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pegasus.Entities.Enumerations;
using Pegasus.Entities.Sorters.ProjectTask;
using Pegasus.Library.Models;
using Pegasus.Models.Settings;
using Pegasus.Models.TaskList;

namespace PegasusTests
{
    class IndexViewModelTests
    {
        private readonly ISettingsModel _settingsModel = new SettingsModel();

        [SetUp]
        public void TestSetup()
        {
            _settingsModel.PaginationDisabled = false;
        }


        [Test]
        public void IndexViewModel_SortsByPriorityDateDesc_ReturnsHighestPriorityFirst()
        {
            var taskList = new List<TaskModel>
            {
                new TaskModel { TaskPriorityId = (int)TaskPriorityEnum.Low },
                new TaskModel { TaskPriorityId = (int)TaskPriorityEnum.Critical }
            };

            var sut = new IndexViewModel(taskList, (int)TaskFilters.All, _settingsModel)
            {
                Sorter = new PriorityDescSorter()
            };

            Assert.IsNotEmpty(sut.ProjectTasks);
            Assert.AreEqual(sut.ProjectTasks.ToArray()[0].TaskPriorityId, (int)TaskPriorityEnum.Critical);
        }

        [Test]
        public void ProjectTask_SortsByModifiedDate_ReturnsMostRecentModifiedFirst()
        {
            var taskList = new List<TaskModel>
            {
                new TaskModel { Modified = new DateTime(2020, 1, 1) },
                new TaskModel { Modified = new DateTime(2020, 2, 1) }
            };

            var sut = new IndexViewModel(taskList, (int)TaskFilters.All, _settingsModel)
            {
                Sorter = new ModifiedDescSorter()
            };

            Assert.IsNotEmpty(sut.ProjectTasks);
            Assert.AreEqual(sut.ProjectTasks.ToArray()[0].Modified, new DateTime(2020, 2, 1));
        }

        [Test]
        public void ProjectTask_NoSorterSpecified_ReturnsMostRecentModifiedFirst()
        {
            var taskList = new List<TaskModel>
            {
                new TaskModel { Modified = new DateTime(2020, 1, 1) },
                new TaskModel { Modified = new DateTime(2020, 2, 1) }
            };

            var sut = new IndexViewModel(taskList, (int)TaskFilters.All, _settingsModel);

            Assert.IsNotEmpty(sut.ProjectTasks);
            Assert.AreEqual(sut.ProjectTasks.ToArray()[0].Modified, new DateTime(2020, 2, 1));
        }
    }
}
