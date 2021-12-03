using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Pegasus.Controllers;
using Pegasus.Entities.Enumerations;
using Pegasus.Library.Api;
using Pegasus.Library.Models;
using Pegasus.Library.Services.Resources;
using Pegasus.Models.TaskList;
using Pegasus.Services;
using Pegasus.Services.Models;

namespace PegasusTests.Controllers
{
    class TaskListControllerTests
    {
        private readonly SettingsModel _settingsModel = new SettingsModel();
        private  Mock<ICommentsEndpoint> _mockCommentsEndpoint;

        private  Mock<IProjectsEndpoint> _mockProjectsEndpoint;
        private  Mock<ISettingsService> _mockSettingsService;
        private  Mock<ITaskFilterService> _mockTaskFilterService;
        private  Mock<ITasksEndpoint> _mockTasksEndpoint;
        private Mock<IMarketingService> _mockMarketingService;
        private Mock<IAnalyticsService> _mockAnalyticsService;


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockMarketingService = new Mock<IMarketingService>();
            _mockAnalyticsService = new Mock<IAnalyticsService>();
        }

        [SetUp]
        public void TestSetup()
        {
            _mockCommentsEndpoint = new Mock<ICommentsEndpoint>();
            _mockProjectsEndpoint = new Mock<IProjectsEndpoint>();
            _mockSettingsService = new Mock<ISettingsService>();
            _mockTaskFilterService = new Mock<ITaskFilterService>();
            _mockTasksEndpoint = new Mock<ITasksEndpoint>();
            _settingsModel.PageSize = 15;
        }

       
        [Test]
        public async Task GET_TaskList_Create_ReturnsViewResult()
        {
            var parentTaskId = 10;
            var projectId = 123;
            var name = "test-project";
            var prefix = "tst";

            SetupMockMethodCalls();

            _mockSettingsService.Setup(x => x.GetSetting<int>("ProjectId")).Returns(projectId);
            _mockProjectsEndpoint.Setup(x => x.GetProject(It.IsAny<int>())).ReturnsAsync(new ProjectModel {Id = projectId, Name=name, ProjectPrefix = prefix});

            var sut = GetController();
            var result = await sut.Create(parentTaskId);

            _mockSettingsService.Verify(x => x.GetSetting<int>(It.IsAny<string>()), Times.Exactly(1));
            _mockProjectsEndpoint.Verify(x => x.GetProject(It.IsAny<int>()), Times.Exactly(1));

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<TaskViewModel>(((ViewResult)result).Model);

            var model = (TaskViewModel)((ViewResult)result).Model;

            Assert.AreEqual(parentTaskId, model.ProjectTask.ParentTaskId);
            Assert.AreEqual(0, model.ProjectTask.Id);
            Assert.AreEqual(projectId, model.Project.Id);
            Assert.AreEqual("tst-<tbc>", model.ProjectTask.TaskRef);
            Assert.AreEqual(name, model.Project.Name);
        }

        [Test]
        public async Task POST_Create_InvalidModel_ReturnsViewResult()
        {
            SetupMockMethodCalls();

            var sut = GetController();
            sut.ModelState.AddModelError(string.Empty, string.Empty);
            var result = await sut.Create(new TaskModel());

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<TaskViewModel>(((ViewResult)result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task POST_Create_ReturnsViewResult()
        {
            var taskId = 543;
            var list = new List<ClaimsIdentity> { new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "user-id") }) };

            SetupMockMethodCalls();
            _mockTasksEndpoint.Setup(x => x.AddTask(It.IsAny<TaskModel>())).ReturnsAsync(taskId);

            var sut = GetController();
            sut.HttpContext.User.AddIdentities(list);

            var result = await sut.Create(new TaskModel());

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual("Edit", ((RedirectToActionResult)result).ActionName);
            Assert.AreEqual(taskId, ((RedirectToActionResult)result).RouteValues["id"]);
            Assert.Zero(sut.ModelState.ErrorCount, "Error count failed.");
        }

        [Test]
        public async Task GET_Edit_ProjectTaskIsNull_ReturnsRedirectToAction()
        {
            _mockTasksEndpoint.Setup(x => x.GetTask(It.IsAny<int>())).ReturnsAsync((TaskModel)null);

            var sut = GetController();
            var result = await sut.Edit("0");

            _mockTasksEndpoint.Verify(x => x.GetTask(It.IsAny<int>()), Times.Exactly(1));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task GET_Edit_IsAjaxRequest_ReturnsPartialView()
        {
            SetupMockMethodCalls();
            _mockSettingsService.Setup(x => x.SaveSettings());

            var sut = GetController();
            sut.HttpContext.Request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            var result = await sut.Edit("0");

            _mockTasksEndpoint.Verify(x => x.GetTask(It.IsAny<int>()), Times.Exactly(2));
            _mockSettingsService.Verify(x => x.SaveSettings(), Times.Exactly(1));
            Assert.IsInstanceOf<PartialViewResult>(result);
            Assert.AreEqual("_EditTaskContent", ((PartialViewResult)result).ViewName);
        }

        [Test]
        public async Task GET_EditWithIntString_IsNotAjaxRequest_ReturnsViewResult()
        {
            SetupMockMethodCalls();
            _mockSettingsService.Setup(x => x.SaveSettings());

            var sut = GetController();
            var result = await sut.Edit("0");

            _mockTasksEndpoint.Verify(x => x.GetTask(It.IsAny<int>()), Times.Exactly(2));
            _mockSettingsService.Verify(x => x.SaveSettings(), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<TaskViewModel>(((ViewResult)result).Model);
        }

        [Test]
        public async Task GET_EditWithString_IsNotAjaxRequest_ReturnsViewResult()
        {
            SetupMockMethodCalls();
            _mockSettingsService.Setup(x => x.SaveSettings());

            var sut = GetController();
            var result = await sut.Edit("TST-01");

            _mockTasksEndpoint.Verify(x => x.GetTask(It.IsAny<int>()), Times.Exactly(1));
            _mockTasksEndpoint.Verify(x => x.GetTaskByRef(It.IsAny<string>()), Times.Exactly(1));
            _mockSettingsService.Verify(x => x.SaveSettings(), Times.Exactly(1));
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<TaskViewModel>(((ViewResult)result).Model);
        }



        [TestCaseSource(typeof(TestData), nameof(TestData.CommentSortOrder))]
        public async Task<int?> GET_Edit_CommentSortOrder_ReturnsViewResultCorrectOrder(int sortOrder)
        {
            var list = new List<TaskCommentModel>
            {
                new TaskCommentModel {Id = 1, Created = DateTime.Now.AddHours(1)},
                new TaskCommentModel {Id = 2, Created = DateTime.Now.AddHours(2)},
                new TaskCommentModel {Id = 3, Created = DateTime.Now}
            };

            SetupMockMethodCalls();
            _settingsModel.CommentSortOrder = sortOrder;
            _mockSettingsService.Setup(x => x.SaveSettings());
            _mockCommentsEndpoint.Setup(x => x.GetComments(It.IsAny<int>())).ReturnsAsync(list);

            var sut = GetController();
            var result = await sut.Edit("0");

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<TaskViewModel>(((ViewResult)result).Model);
            return ((TaskViewModel)((ViewResult)result).Model)?.Comments?.Comments.FirstOrDefault()?.Id;
        }
        
        [Test]
        public async Task POST_Edit_InvalidModel_ReturnsViewResultWithErrors()
        {
            SetupMockMethodCalls();

            var sut = GetController();
            sut.ModelState.AddModelError(string.Empty, string.Empty);
            var result = await sut.Edit(new TaskModel(), 0, string.Empty, new List<TaskCommentModel>(), string.Empty);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<TaskViewModel>(((ViewResult)result).Model);
            Assert.NotZero(sut.ModelState.ErrorCount);
        }

        [Test]
        public async Task POST_Edit_UpdateTask_ReturnsRedirectToAction()
        {
            SetupMockMethodCalls();
            _mockTasksEndpoint.Setup(x => x.UpdateTask(It.IsAny<TaskModel>()));
            _mockCommentsEndpoint.Setup(x => x.UpdateComments(It.IsAny <List<TaskCommentModel>>()));

            var sut = GetController();
            var result = await sut.Edit(new TaskModel{Id=54321}, 0, string.Empty, new List<TaskCommentModel>(), string.Empty);

            _mockTasksEndpoint.Verify(x => x.UpdateTask(It.IsAny<TaskModel>()), Times.Exactly(1));
            _mockCommentsEndpoint.Verify(x => x.UpdateComments(It.IsAny <List<TaskCommentModel>>()), Times.Exactly(1));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual("Edit", ((RedirectToActionResult)result).ActionName);
            Assert.AreEqual(54321, ((RedirectToActionResult)result).RouteValues["id"]);
        }

        [Test]
        public async Task POST_Edit_HasNewComment_ExecutesAddNewComment()
        {
            SetupMockMethodCalls();
            _mockTasksEndpoint.Setup(x => x.UpdateTask(It.IsAny<TaskModel>()));
            _mockCommentsEndpoint.Setup(x => x.UpdateComments(It.IsAny <List<TaskCommentModel>>()));
            _mockCommentsEndpoint.Setup(x => x.AddComment(It.IsAny<TaskCommentModel>()));

            var sut = GetController();
            _ = await sut.Edit(new TaskModel(), 0, "A new comment", new List<TaskCommentModel>(), string.Empty);

            _mockCommentsEndpoint.Verify(x => x.AddComment(It.IsAny<TaskCommentModel>()), Times.Exactly(1));
        }

        [Test]
        public async Task POST_Edit_HasNoNewComment_DoesNotExecuteAddNewComment()
        {
            SetupMockMethodCalls();
            _mockTasksEndpoint.Setup(x => x.UpdateTask(It.IsAny<TaskModel>()));
            _mockCommentsEndpoint.Setup(x => x.UpdateComments(It.IsAny <List<TaskCommentModel>>()));
            _mockCommentsEndpoint.Setup(x => x.AddComment(It.IsAny<TaskCommentModel>()));

            var sut = GetController();
            _ = await sut.Edit(new TaskModel(), 0, null, new List<TaskCommentModel>(), string.Empty);

            _mockCommentsEndpoint.Verify(x => x.AddComment(It.IsAny<TaskCommentModel>()), Times.Never);
        }

        [Test]
        public async Task POST_Edit_AddNewTask_ReturnsRedirectToActionCreate()
        {
            SetupMockMethodCalls();
            _mockTasksEndpoint.Setup(x => x.UpdateTask(It.IsAny<TaskModel>()));
            _mockCommentsEndpoint.Setup(x => x.UpdateComments(It.IsAny <List<TaskCommentModel>>()));

            var sut = GetController();
            var result = await sut.Edit(new TaskModel(), 0, null, new List<TaskCommentModel>(), "123");

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual("Create", ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task POST_Edit_IsClosedWithParent_ReturnsRedirectToActionParentTask()
        {
            SetupMockMethodCalls();
            _mockTasksEndpoint.Setup(x => x.UpdateTask(It.IsAny<TaskModel>()));
            _mockCommentsEndpoint.Setup(x => x.UpdateComments(It.IsAny <List<TaskCommentModel>>()));

            var sut = GetController();
            var result = await sut.Edit(new TaskModel {ParentTaskId = 123, TaskStatusId = (int) TaskStatusEnum.Completed}, 0, null, new List<TaskCommentModel>(), null);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual("Edit", ((RedirectToActionResult)result).ActionName);
            Assert.AreEqual(123, ((RedirectToActionResult)result).RouteValues["id"]);
        }

        [Test]
        public async Task POST_Edit_IsClosedWithoutParent_ReturnsRedirectToActionParentTask()
        {
            SetupMockMethodCalls();
            _mockTasksEndpoint.Setup(x => x.UpdateTask(It.IsAny<TaskModel>()));
            _mockCommentsEndpoint.Setup(x => x.UpdateComments(It.IsAny <List<TaskCommentModel>>()));

            var sut = GetController();
            var result = await sut.Edit(new TaskModel {TaskStatusId = (int) TaskStatusEnum.Completed}, 0, null, new List<TaskCommentModel>(), null);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task POST_Edit_IsClosedWithOpenSubTasks_ReturnsViewResultWithBannerWarning()
        {
            SetupMockMethodCalls();
            _mockTasksEndpoint.Setup(x => x.UpdateTask(It.IsAny<TaskModel>()));
            _mockCommentsEndpoint.Setup(x => x.UpdateComments(It.IsAny <List<TaskCommentModel>>()));
            _mockTasksEndpoint.Setup(x => x.GetSubTasks(It.IsAny<int>())).ReturnsAsync(new List<TaskModel>()
                { new TaskModel { TaskStatusId = (int)TaskStatusEnum.InProgress } });

            var sut = GetController();
            var result = await sut.Edit(new TaskModel {TaskStatusId = (int) TaskStatusEnum.Completed}, 0, null, new List<TaskCommentModel>(), null);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<TaskViewModel>(((ViewResult)result).Model);
            Assert.AreEqual(Resources.ControllerStrings.TaskListController.CannotCloseWithOpenSubTasks, ((TaskViewModel)((ViewResult)result).Model).BannerMessage);
        }

        [Test]
        public void GET_Error_ReturnsViewResult()
        {
            var sut = GetController();
            var result = sut.Error();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task GET_Index_IsAjaxRequest_ReturnsPartialViewResult()
        {
            SetupMockMethodCalls();
            _mockTasksEndpoint.Setup(x => x.GetTasks(It.IsAny<int>())).ReturnsAsync(new List<TaskModel>());
            _mockTasksEndpoint.Setup(x => x.GetAllTasks()).ReturnsAsync(new List<TaskModel>());

            var sut = GetController();
            sut.HttpContext.Request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            var result = await sut.Index();

            Assert.IsInstanceOf<PartialViewResult>(result);
            Assert.IsInstanceOf<IndexViewModel>(((PartialViewResult)result).Model);
            Assert.AreEqual("../TaskList/_ProjectTaskList", ((PartialViewResult)result).ViewName);
        }

        [Test]
        public async Task GET_Index_IsNotAjaxRequest_ReturnsPartialViewResult()
        {
            SetupMockMethodCalls();
            _mockTasksEndpoint.Setup(x => x.GetTasks(It.IsAny<int>())).ReturnsAsync(new List<TaskModel>());
            _mockTasksEndpoint.Setup(x => x.GetAllTasks()).ReturnsAsync(new List<TaskModel>());

            var sut = GetController();
            var result = await sut.Index();

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<IndexViewModel>(((ViewResult)result).Model);
            Assert.AreEqual("../TaskList/Index", ((ViewResult)result).ViewName);
        }

        private void SetupMockMethodCalls()
        {
            _mockTasksEndpoint.Setup(x=>x.GetAllTaskPriorities()).ReturnsAsync(new List<TaskPriorityModel>());
            _mockTasksEndpoint.Setup(x=>x.GetAllTaskStatuses()).ReturnsAsync(new List<TaskStatusModel>());
            _mockTasksEndpoint.Setup(x=>x.GetAllTaskTypes()).ReturnsAsync(new List<TaskTypeModel>());
            _mockTasksEndpoint.Setup(x => x.GetTask(It.IsAny<int>())).ReturnsAsync(new TaskModel());
            _mockTasksEndpoint.Setup(x => x.GetTaskByRef(It.IsAny<string>())).ReturnsAsync(new TaskModel());
            _mockTasksEndpoint.Setup(x => x.GetSubTasks(It.IsAny<int>())).ReturnsAsync(new List<TaskModel>());
            _mockCommentsEndpoint.Setup(x => x.GetComments(It.IsAny<int>())).ReturnsAsync(new List<TaskCommentModel>());
            _mockProjectsEndpoint.Setup(x => x.GetProject(It.IsAny<int>())).ReturnsAsync(new ProjectModel());
        }
       
        private TaskListController GetController()
        {
            var controllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            _mockSettingsService.Setup(x => x.Settings).Returns(_settingsModel);
            var settingsService = _mockSettingsService.Object;
            return new TaskListController(_mockTaskFilterService.Object, _mockProjectsEndpoint.Object, _mockTasksEndpoint.Object, 
                _mockCommentsEndpoint.Object, settingsService, _mockMarketingService.Object, _mockAnalyticsService.Object)
            {
                ControllerContext = controllerContext
            };
        }

        class TestData
        {
            public static IEnumerable CommentSortOrder
            {
                get
                {
                    yield return new TestCaseData((int)CommentSortOrderEnum.DateAscending).Returns(3);
                    yield return new TestCaseData((int)CommentSortOrderEnum.DateDescending).Returns(2);
                    yield return new TestCaseData(-1).Returns(3);
                }
            }
        }
    }
}
