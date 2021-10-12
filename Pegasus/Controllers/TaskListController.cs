using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pegasus.Entities.Attributes;
using Pegasus.Entities.Enumerations;
using Pegasus.Extensions;
using Pegasus.Library.Api;
using Pegasus.Library.Models;
using Pegasus.Models;
using Pegasus.Models.TaskList;
using Pegasus.Services;
using TaskListControllerStrings = Pegasus.Library.Services.Resources.Resources.ControllerStrings.TaskListController;

namespace Pegasus.Controllers
{
    [Authorize(Roles = "PegasusUser")]
    [Require2Fa]
    public class TaskListController : Controller
    {
        private readonly ICommentsEndpoint _commentsEndpoint;
        private readonly int _pageSize;
        private readonly IProjectsEndpoint _projectsEndpoint;
        private readonly ISettingsService _settingsService;
        private readonly ITaskFilterService _taskFilterService;
        private readonly ITasksEndpoint _tasksEndpoint;

        public TaskListController(ITaskFilterService taskFilterService,
            IProjectsEndpoint projectsEndpoint, ITasksEndpoint tasksEndpoint,
            ICommentsEndpoint commentsEndpoint, ISettingsService settingsService,
            IMarketingService marketingService, IAnalyticsService analyticsService)
        {
            _taskFilterService = taskFilterService;
            _projectsEndpoint = projectsEndpoint;
            _tasksEndpoint = tasksEndpoint;
            _commentsEndpoint = commentsEndpoint;
            _settingsService = settingsService;
            _pageSize = settingsService.Settings.PageSize;

            // this is here to simulate stored data, for cookie policy interaction.
            marketingService.SaveMarketingData("Some Marketing Data");
            analyticsService.SaveAnalyticsData("Some Analytics Data");
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            var projectId = _settingsService.GetSetting<int>(nameof(_settingsService.Settings.ProjectId));
            var project = await _projectsEndpoint.GetProject(projectId);
            var taskModel = new TaskModel
            {
                ProjectId = projectId,
                TaskRef = $"{project.ProjectPrefix}-<tbc>",
                ParentTaskId = id
            };
            var model = await CreateTaskViewModel(new TaskViewModelArgs
            {
                ProjectTask = taskModel,
                Project = project
            });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Description,Name,ProjectId,ParentTaskId,TaskRef,TaskStatusId,TaskTypeId,TaskPriorityId,FixedInRelease")]
            TaskModel projectTask)
        {
            if (!ModelState.IsValid)
            {
                var model = await CreateTaskViewModel(new TaskViewModelArgs
                {
                    ProjectTask = projectTask
                });

                return View(model);
            }

            projectTask.UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var taskId = await _tasksEndpoint.AddTask(projectTask);
            return RedirectToAction("Edit", new { id = taskId});
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var projectTask = await _tasksEndpoint.GetTask(id);
            if (projectTask == null)
            {
                return RedirectToAction("Index");
            }
            _settingsService.Settings.ProjectId = projectTask.ProjectId;
            _settingsService.SaveSettings();

            var model = await CreateTaskViewModel(new TaskViewModelArgs
            {
                ProjectTask = projectTask
            });

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

            if (!ModelState.IsValid)
            {
                var model = await CreateTaskViewModel(taskViewModelArgs);
                return View(model);
            }

            var (isValidClose, actionResult1) = await IsClosingTaskWithOpenSubTasks(projectTask, taskViewModelArgs);
            if (!isValidClose)
                return actionResult1;

            await UpdateProjectTaskData(projectTask, newComment, comments);

            if (!string.IsNullOrWhiteSpace(addSubTask))
            {
                return RedirectToAction("Create", new { id = addSubTask});
            }

            var (isClosed, actionResult) = IsTaskClosed(projectTask, currentTaskStatus);
            if (isClosed)
                return actionResult;
                    
            return RedirectToAction("Edit", new { id = (int?)projectTask.Id} );
        }

        public IActionResult Error()
        {
            var model = new BaseViewModel {ProjectId = 0};
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var taskFilterId = _settingsService.GetSetting<int>(nameof(_settingsService.Settings.TaskFilterId));
            var projectId = _settingsService.GetSetting<int>(nameof(_settingsService.Settings.ProjectId));
            var page = GetPage();

            var project = await _projectsEndpoint.GetProject(projectId) ?? new ProjectModel {Id = 0, Name = "All"};
            var projectTasks = project.Id > 0
                ? await _tasksEndpoint.GetTasks(project.Id)
                : await _tasksEndpoint.GetAllTasks();

            var model = new IndexViewModel(projectTasks, taskFilterId, _settingsService.Settings)
            {
                ProjectId = project.Id,
                Page = page,
                PageSize = _pageSize,
                Projects = await _projectsEndpoint.GetAllProjects(),
                TaskFilters = _taskFilterService.GetTaskFilters(),
                Project = project
            };

            if (Request != null && Request.IsAjaxRequest()) return PartialView("../TaskList/_ProjectTaskList", model);

            return View("../TaskList/Index", model);
        }


        private async Task<TaskViewModel> CreateTaskViewModel(TaskViewModelArgs args)
        {
            var taskProperties = new TaskPropertiesViewModel
            {
                ProjectTask = args.ProjectTask,
                TaskPriorities = new SelectList(await _tasksEndpoint.GetAllTaskPriorities(), "Id", "Name", 1),
                TaskStatuses = new SelectList(await _tasksEndpoint.GetAllTaskStatuses(), "Id", "Name", 1),
                TaskTypes = new SelectList(await _tasksEndpoint.GetAllTaskTypes(), "Id", "Name", 1)
            };

            var model = new TaskViewModel
            {
                BannerMessage = args.BannerMessage,
                Comments =  await GetComments(args.Comments, args.ProjectTask.Id),
                CurrentTaskStatus = args.CurrentStatusId != 0 ? args.CurrentStatusId : args.ProjectTask.TaskStatusId,
                NewComment = args.NewComment,
                ParentTask = await _tasksEndpoint.GetTask(args.ProjectTask.ParentTaskId ?? 0),
                Project = args.Project ?? await _projectsEndpoint.GetProject(args.ProjectTask.ProjectId),
                ProjectId = args.ProjectTask.ProjectId,
                ProjectTask = args.ProjectTask,
                SubTasks = await _tasksEndpoint.GetSubTasks(args.ProjectTask.Id),
                TaskProperties = taskProperties
            };

            return model;
        }

        private async Task<CommentsViewModel> GetComments(IEnumerable<TaskCommentModel> comments, int projectTaskId)
        {
            comments ??= await _commentsEndpoint.GetComments(projectTaskId);

            return new CommentsViewModel
            {
                Comments = _settingsService.Settings.CommentSortOrder switch
                {
                    (int)CommentSortOrderEnum.DateDescending => comments.OrderByDescending(x => x.Created).ToList(),
                    _ => comments.OrderBy(x => x.Created).ToList()
                }
            };
        }

        private int GetPage()
        {
            const int defaultPageNo = 1;
            var qsPage = Request.Query["page"];
            return int.TryParse(qsPage, out var pageNo) ? pageNo : defaultPageNo;
        }

        private async Task<bool> HasIncompleteSubTasks(int taskId)
        {
            var subTasks = await _tasksEndpoint.GetSubTasks(taskId);
            return subTasks.Any(subTask => !subTask.IsClosed());
        }

        private (bool isClosed, IActionResult actionResult) IsTaskClosed(TaskModel projectTask, int currentTaskStatus)
        {
            if (projectTask.IsClosed() && projectTask.TaskStatusId != currentTaskStatus)
            {
                if (projectTask.HasParentTask())
                {
                    // return to the parent task
                    return (true, RedirectToAction("Edit", new { id = projectTask.ParentTaskId}));
                }

                // return to the main index page
                return (true, RedirectToAction("Index"));
            }

            return (false, null);
        }

        private async Task<(bool isValidClose, IActionResult actionResult)> IsClosingTaskWithOpenSubTasks(TaskModel projectTask, TaskViewModelArgs taskViewModelArgs)
        {
            if (projectTask.IsClosed() && await HasIncompleteSubTasks(projectTask.Id))
            {
                taskViewModelArgs.BannerMessage = TaskListControllerStrings.CannotCloseWithOpenSubTasks;

                var model1 = await CreateTaskViewModel(taskViewModelArgs);
                return (false, View(model1));
            }

            return (true, null);
        }

        private async Task UpdateComments(string newComment, IList<TaskCommentModel> comments, TaskModel projectTask)
        {
            await _commentsEndpoint.UpdateComments(comments.ToList());
            if (!string.IsNullOrWhiteSpace(newComment))
                await _commentsEndpoint.AddComment(new TaskCommentModel
                    {TaskId = projectTask.Id, Comment = newComment, UserId = projectTask.UserId});
        }

        private async Task UpdateProjectTaskData(TaskModel projectTask, string newComment, IList<TaskCommentModel> comments)
        {
            projectTask.UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _tasksEndpoint.UpdateTask(projectTask);
            await UpdateComments(newComment, comments, projectTask);
        }

    }
}