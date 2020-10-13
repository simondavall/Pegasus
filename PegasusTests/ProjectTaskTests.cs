using NUnit.Framework;
using Pegasus.Entities;
using Pegasus.Entities.Enumerations;
using Pegasus.Entities.Sorters.ProjectTask;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PegasusTests
{
    class ProjectTaskTests
    {
        [Test]
        public void ProjectTask_SortsByModifiedDate_ReturnsMostRecentModifiedFirst()
        {
            var taskList = new List<ProjectTaskExt>
            {
                new ProjectTaskExt(new ProjectTask { Modified = new DateTime(2020, 1, 1) }),
                new ProjectTaskExt(new ProjectTask { Modified = new DateTime(2020, 2, 1) })
            };

            var sut = new Pegasus.Models.Home.IndexViewModel(taskList);
            var sorter = new ModifiedDescSorter();

            Assert.IsNotEmpty(sut.ProjectTasks);
            Assert.AreEqual(sut.ProjectTasks.Sorted(sorter).ToArray()[0].Modified, new DateTime(2020, 2, 1));
        }

        [Test]
        public void ProjectTask_SortsByPriorityDateDesc_ReturnsHighestPriorityFirst()
        {
            var taskList = new List<ProjectTaskExt>
            {
                new ProjectTaskExt(new ProjectTask { TaskPriorityId = (int)TaskPriorityEnum.Low }),
                new ProjectTaskExt(new ProjectTask { TaskPriorityId = (int)TaskPriorityEnum.Critical })
            };
        
            var sut = new Pegasus.Models.Home.IndexViewModel(taskList);
            var sorter = new PriorityDescSorter();

            Assert.IsNotEmpty(sut.ProjectTasks);
            Assert.AreEqual(sut.ProjectTasks.Sorted(sorter).ToArray()[0].TaskPriorityId, (int)TaskPriorityEnum.Critical);
        }

        [Test]
        public void ProjectTask_SortsByPriorityDateAsc_ReturnsLowestPriorityFirst()
        {
            var taskList = new List<ProjectTaskExt>
            {
                new ProjectTaskExt(new ProjectTask { TaskPriorityId = (int)TaskPriorityEnum.High }),
                new ProjectTaskExt(new ProjectTask { TaskPriorityId = (int)TaskPriorityEnum.Low }),
                new ProjectTaskExt(new ProjectTask { TaskPriorityId = (int)TaskPriorityEnum.Critical })
            };

            var sut = new Pegasus.Models.Home.IndexViewModel(taskList);
            var sorter = new PriorityAscSorter();

            Assert.IsNotEmpty(sut.ProjectTasks);
            Assert.AreEqual(sut.ProjectTasks.Sorted(sorter).ToArray()[0].TaskPriorityId, (int)TaskPriorityEnum.Low);
        }

        [Test]
        public void ProjectTask_SortsByTaskRefDesc_ReturnsHighestTaskRefFirst()
        {
            var taskList = new List<ProjectTaskExt>
            {
                new ProjectTaskExt(new ProjectTask { TaskRef = "1" }),
                new ProjectTaskExt(new ProjectTask { TaskRef = "2" }),
                new ProjectTaskExt(new ProjectTask { TaskRef = "3" })
            };
        
            var sut = new Pegasus.Models.Home.IndexViewModel(taskList);
            var sorter = new TaskRefDescSorter();
        
            Assert.IsNotEmpty(sut.ProjectTasks);
            Assert.AreEqual(sut.ProjectTasks.Sorted(sorter).ToArray()[0].TaskRef, "3");
        }
    }
}
