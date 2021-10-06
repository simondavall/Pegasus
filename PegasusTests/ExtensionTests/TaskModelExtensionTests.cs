using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pegasus.Entities.Enumerations;
using Pegasus.Entities.Profiles;
using Pegasus.Entities.Sorters.ProjectTask;
using Pegasus.Extensions;
using Pegasus.Library.Models;

namespace PegasusTests.ExtensionTests
{
    class TaskModelExtensionTests
    {
        private List<TaskModel> _taskList;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _taskList = new List<TaskModel>
            {
                new TaskModel {TaskPriorityId = 1, TaskRef = "1", Modified = DateTime.Now },
                new TaskModel {TaskPriorityId = 3, TaskRef = "4", Modified = DateTime.Now.AddDays(1) },
                new TaskModel {TaskPriorityId = 5, TaskRef = "5", Id = 123, Modified = DateTime.Now.AddDays(2)},
                new TaskModel {TaskPriorityId = 3, TaskRef = "2", Modified = DateTime.Now.AddDays(1) },
                new TaskModel {TaskPriorityId = 2, TaskRef = "3", Modified = DateTime.Now.AddDays(1) }
            };
        }

        [TestCaseSource(typeof(TestData), nameof(TestData.IsClosedData))]
        public bool IsClosed_TaskStatus(int taskStatus)
        {
            var model = new TaskModel
            {
                TaskStatusId = taskStatus
            };

            return model.IsClosed();
        }

        [Test]
        public void HasParentTask_NoParentTaskId_ReturnsFalse()
        {
            var model = new TaskModel
            {
                ParentTaskId = null
            };
            Assert.IsFalse(model.HasParentTask());
        }

        [Test]
        public void HasParentTask_ParentTaskId_ReturnsTrue()
        {
            var model = new TaskModel
            {
                ParentTaskId = 123
            };
            Assert.IsTrue(model.HasParentTask());
        }

        [Test]
        public void PriorityIconClass_TaskLowPriority_ReturnsEmptyString()
        {
            var model = new TaskModel
            {
                TaskPriorityId = (int)TaskPriorityEnum.Low
            };

            Assert.AreEqual(string.Empty, model.PriorityIconClass());
        }

        [Test]
        public void PriorityIconClass_TaskHighPriorityCompleted_ReturnsClosedPriorityIcon()
        {
            var model = new TaskModel
            {
                TaskPriorityId = (int)TaskPriorityEnum.High,
                TaskStatusId = (int)TaskStatusEnum.Completed
            };

            Assert.AreEqual("priority-icon-closed", model.PriorityIconClass());
        }

        [Test]
        public void PriorityIconClass_TaskHighPriorityInProgress_ReturnsHighPriorityIcon()
        {
            var model = new TaskModel
            {
                TaskPriorityId = (int)TaskPriorityEnum.High,
                TaskStatusId = (int)TaskStatusEnum.InProgress
            };

            Assert.AreEqual("priority-icon-4", model.PriorityIconClass());
        }

        [Test]
        public void PriorityIconClass_TaskCriticalPriorityInProgress_ReturnsCriticalPriorityIcon()
        {
            var model = new TaskModel
            {
                TaskPriorityId = (int)TaskPriorityEnum.Critical,
                TaskStatusId = (int)TaskStatusEnum.InProgress
            };

            Assert.AreEqual("priority-icon-5", model.PriorityIconClass());
        }

        [TestCaseSource(typeof(TestData), nameof(TestData.TaskProfileData))]
        public string TaskProfile_TaskStatus(int taskStatus)
        {
            var model = new TaskModel
            {
                TaskStatusId = taskStatus
            };

            return model.TaskProfile().ToString();
        }

        [TestCaseSource(typeof(TestData), nameof(TestData.TaskFilterData))]
        public int Filtered_TaskFilter(int taskFilterId)
        {
            var list = new List<TaskModel>();
            for (var j = 1; j <= 5; j++)
            {
                for (var i = 1; i <= 6; i++ )
                {
                    list.Add(new TaskModel { TaskStatusId = i , TaskPriorityId = j });
                }
            }
            
            var sut = list.AsEnumerable().Filtered(taskFilterId);

            return sut.Count();
        }

        [Test]
        public void Sorted_ModifiedDescSorter_ReturnsCorrectSort()
        {
            ISorter sorter = new ModifiedDescSorter();
            var sut = _taskList.Sorted(sorter);

            Assert.AreEqual(123, sut.First().Id);
        }

        [Test]
        public void Sorted_TaskRefDescSorter_ReturnsCorrectSort()
        {
            ISorter sorter = new TaskRefDescSorter();
            var sut = _taskList.Sorted(sorter);

            Assert.AreEqual(123, sut.First().Id);
        }

        [Test]
        public void Sorted_PriorityAscSorter_ReturnsCorrectSort()
        {
            ISorter sorter = new PriorityAscSorter();
            var sut = _taskList.Sorted(sorter);

            Assert.AreEqual(123, sut.Last().Id);
        }

        [Test]
        public void Sorted_PriorityDescSorter_ReturnsCorrectSort()
        {
            ISorter sorter = new PriorityDescSorter();
            var sut = _taskList.Sorted(sorter);

            Assert.AreEqual(123, sut.First().Id);
        }
    }

    class TestData
    {
        public static IEnumerable IsClosedData
        {
            get
            {
                yield return new TestCaseData((int)TaskStatusEnum.Backlog).Returns(false);
                yield return new TestCaseData((int)TaskStatusEnum.Completed).Returns(true);
                yield return new TestCaseData((int)TaskStatusEnum.InProgress).Returns(false);
                yield return new TestCaseData((int)TaskStatusEnum.OnHold).Returns(false);
                yield return new TestCaseData((int)TaskStatusEnum.Obsolete).Returns(true);
                yield return new TestCaseData((int)TaskStatusEnum.Submitted).Returns(false);
            }
        } 

        public static IEnumerable TaskProfileData
        {
            get
            {
                yield return new TestCaseData((int)TaskStatusEnum.Backlog).Returns(new DefaultTaskProfile().ToString());
                yield return new TestCaseData((int)TaskStatusEnum.Completed).Returns(new CompletedTaskProfile().ToString());
                yield return new TestCaseData((int)TaskStatusEnum.InProgress).Returns(new InProgressTaskProfile().ToString());
                yield return new TestCaseData((int)TaskStatusEnum.OnHold).Returns(new OnHoldTaskProfile().ToString());
                yield return new TestCaseData((int)TaskStatusEnum.Obsolete).Returns(new ObsoleteTaskProfile().ToString());
                yield return new TestCaseData((int)TaskStatusEnum.Submitted).Returns(new DefaultTaskProfile().ToString());
            }
        } 
        public static IEnumerable TaskFilterData
        {
            get
            {
                yield return new TestCaseData((int)TaskFilters.Backlog).Returns(5);
                yield return new TestCaseData((int)TaskFilters.All).Returns(25);
                yield return new TestCaseData((int)TaskFilters.HighPriority).Returns(10);
                yield return new TestCaseData((int)TaskFilters.Obsolete).Returns(5);
                yield return new TestCaseData((int)TaskFilters.Open).Returns(15);
            }
        }  
    }
}
