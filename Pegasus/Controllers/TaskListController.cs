using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pegasus.Controllers.Helpers;
using Pegasus.Entities.Attributes;
using Pegasus.Extensions;
using Pegasus.Library.Api;
using Pegasus.Library.Models;
using Pegasus.Models;
using Pegasus.Models.TaskList;
using Pegasus.Services;


namespace Pegasus.Controllers
{
    [Authorize(Roles = "PegasusUser")]
    [Require2Fa]
    public class TaskListController : Controller
    {
        private readonly ITasksEndpoint _tasksEndpoint;
        private readonly TaskListHelperForCreate _helperForCreate;
        private readonly TaskListHelperForEdit _helperForEdit;
        private readonly TaskListHelperForIndex _helperForIndex;

        public TaskListController(ITaskFilterService taskFilterService, IProjectsEndpoint projectsEndpoint, 
            ITasksEndpoint tasksEndpoint, ICommentsEndpoint commentsEndpoint, ISettingsService settingsService,
            IMarketingService marketingService, IAnalyticsService analyticsService)
        {
            _tasksEndpoint = tasksEndpoint;

            var args = new TaskListHelperArgs(this, tasksEndpoint, settingsService, 
                projectsEndpoint, commentsEndpoint, taskFilterService);
            
            _helperForCreate = new TaskListHelperForCreate(args);
            _helperForEdit = new TaskListHelperForEdit(args);
            _helperForIndex = new TaskListHelperForIndex(args);
            
#if DEBUG
            // this is here to simulate stored data, for cookie policy interaction.
            marketingService.SaveMarketingData("Some Marketing Data");
            analyticsService.SaveAnalyticsData("Some Analytics Data");
#endif
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            var args = await _helperForCreate.GetTaskViewModelArgs(id);
            var model = await _helperForCreate.CreateTaskViewModel(args);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Description,Name,ProjectId,ParentTaskId,TaskRef,TaskStatusId,TaskTypeId,TaskPriorityId,FixedInRelease")]
            TaskModel projectTask)
        {
            var (isValid, actionResult) = await _helperForCreate.IsDataValid(projectTask);
            if (!isValid)
                return actionResult;
            
            var taskId = await _helperForCreate.AddProjectTask(projectTask);
            
            return RedirectToAction("Edit", new { id = taskId});
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var projectTask = await _tasksEndpoint.GetTask(id);
            if (projectTask == null)
            {
                return RedirectToAction("Index", "TaskList");
            }
            
            _helperForEdit.UpdateSettingsWithCurrentProject(projectTask);
            var model = await _helperForEdit.CreateModel(projectTask);
            
            if (Request != null && Request.IsAjaxRequest())
            {
                return PartialView("_EditTaskContent", model);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [Bind(
                "Id,Description,Name,Created,ProjectId,ParentTaskId,TaskRef,TaskStatusId,TaskTypeId,TaskPriorityId,FixedInRelease")]
            TaskModel projectTask,
            int currentTaskStatus, string newComment, [Bind("Id,Comment")] IList<TaskCommentModel> comments, string addSubTask)
        {
            var taskViewModelArgs = new TaskViewModelArgs
            {
                ProjectTask = projectTask,
                CurrentStatusId = currentTaskStatus,
                Comments = comments,
                NewComment = newComment
            };
            
            var (isValidData, dataInvalidActionResult) = await _helperForEdit.IsDataValid(projectTask, taskViewModelArgs);
            if (!isValidData)
                return dataInvalidActionResult;
            
            await _helperForEdit.UpdateProjectTaskData(projectTask, newComment, comments);
            
            return _helperForEdit.RedirectFromEdit(projectTask, addSubTask, currentTaskStatus);
        }

        public IActionResult Error()
        {
            var model = new BaseViewModel {ProjectId = 0};
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await  _helperForIndex.GetIndexViewModel();

            if (Request != null && Request.IsAjaxRequest())
            {
                return PartialView("../TaskList/_ProjectTaskList", model);
            }

            return View("../TaskList/Index", model);
        }
    }
}