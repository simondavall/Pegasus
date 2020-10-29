using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pegasus.Entities;
using Pegasus.Entities.Enumerations;
using Pegasus.Entities.Sorters.ProjectTask;
using Pegasus.Library.Models;
using Pegasus.Models.TaskList;

namespace PegasusTests
{
    class IndexViewModelTests
    {
        [Test]
        public void IndexViewModel_SortsByPriorityDateDesc_ReturnsHighestPriorityFirst()
        {
            var taskList = new List<ProjectTaskExt>
            {
                new ProjectTaskExt(new TaskModel { TaskPriorityId = (int)TaskPriorityEnum.Low }),
                new ProjectTaskExt(new TaskModel { TaskPriorityId = (int)TaskPriorityEnum.Critical })
            };

            var sut = new IndexViewModel(taskList)
            {
                Sorter = new PriorityDescSorter()
            };

            Assert.IsNotEmpty(sut.ProjectTasks);
            Assert.AreEqual(sut.ProjectTasks.ToArray()[0].TaskPriorityId, (int)TaskPriorityEnum.Critical);
        }

        [Test]
        public void ProjectTask_SortsByModifiedDate_ReturnsMostRecentModifiedFirst()
        {
            var taskList = new List<ProjectTaskExt>
            {
                new ProjectTaskExt(new TaskModel { Modified = new DateTime(2020, 1, 1) }),
                new ProjectTaskExt(new TaskModel { Modified = new DateTime(2020, 2, 1) })
            };

            var sut = new IndexViewModel(taskList)
            {
                Sorter = new ModifiedDescSorter()
            };

            Assert.IsNotEmpty(sut.ProjectTasks);
            Assert.AreEqual(sut.ProjectTasks.ToArray()[0].Modified, new DateTime(2020, 2, 1));
        }

        [Test]
        public void ProjectTask_NoSorterSpecified_ReturnsMostRecentModifiedFirst()
        {
            var taskList = new List<ProjectTaskExt>
            {
                new ProjectTaskExt(new TaskModel { Modified = new DateTime(2020, 1, 1) }),
                new ProjectTaskExt(new TaskModel { Modified = new DateTime(2020, 2, 1) })
            };

            var sut = new IndexViewModel(taskList);

            Assert.IsNotEmpty(sut.ProjectTasks);
            Assert.AreEqual(sut.ProjectTasks.ToArray()[0].Modified, new DateTime(2020, 2, 1));
        }
    }
}
