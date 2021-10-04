using System;
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
using Pegasus.Services.Models;

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
            _pageSize = settingsService.PageSize;

            // this is here to simulate stored data, for cookie policy interaction.
            marketingService.SaveMarketingData("Some Marketing Data");
            analyticsService.SaveAnalyticsData("Some Analytics Data");
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            var projectId = _settingsService.GetSetting<int>(nameof(_settingsService.ProjectId));
            var project = await _projectsEndpoint.GetProject(projectId);
            var taskModel = new TaskModel
            {
                ProjectId = projectId,
                TaskRef = $"{project.ProjectPrefix}-<tbc>",
                ParentTaskId = id
            };
            var model = await TaskViewModel.Create(new TaskViewModelArgs
            {
                ProjectsEndpoint = _projectsEndpoint,
                TasksEndpoint = _tasksEndpoint,
                CommentsEndpoint = _commentsEndpoint,
                ProjectTask = taskModel,
                Project = project
            });

            model.ProjectTask = taskModel;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Description,Name,ProjectId,ParentTaskId,TaskRef,TaskStatusId,TaskTypeId,TaskPriorityId,FixedInRelease")]
            TaskModel projectTask)
        {
            projectTask.Created = projectTask.Modified = DateTime.Now;
            if (ModelState.IsValid)
            {
                projectTask.UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var taskId = await _tasksEndpoint.AddTask(projectTask);
                return RedirectToAction("Edit", new { id = taskId});
            }

            var model = await TaskViewModel.Create(new TaskViewModelArgs
            {
                ProjectsEndpoint = _projectsEndpoint,
                TasksEndpoint = _tasksEndpoint,
                CommentsEndpoint = _commentsEndpoint,
                ProjectTask = projectTask
            });

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var projectTask = await _tasksEndpoint.GetTask(id);
            if (projectTask == null)
            {
                //todo set a banner message, project task not found
                return RedirectToAction("Index");
            }
            _settingsService.ProjectId = projectTask.ProjectId;
            _settingsService.SaveSettings();

            var model = await TaskViewModel.Create(new TaskViewModelArgs
            {
                ProjectsEndpoint = _projectsEndpoint,
                TasksEndpoint = _tasksEndpoint,
                CommentsEndpoint = _commentsEndpoint,
                ProjectTask = projectTask
            });

            if (Request != null && Request.IsAjaxRequest()) return PartialView("_EditTaskContent", model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [Bind(
                "Id,Description,Name,Created,ProjectId,ParentTaskId,TaskRef,TaskStatusId,TaskTypeId,TaskPriorityId,FixedInRelease")]
            TaskModel projectTask,
            int existingTaskStatus, string newComment, [Bind("Id,Comment")] IList<TaskCommentModel> comments, string addSubTask)
        {
            var taskViewModelArgs = new TaskViewModelArgs
            {
                ProjectsEndpoint = _projectsEndpoint,
                TasksEndpoint = _tasksEndpoint,
                CommentsEndpoint = _commentsEndpoint,
                ProjectTask = projectTask,
                ExistingStatusId = existingTaskStatus,
                Comments = comments,
                NewComment = newComment
            };
            
            if (ModelState.IsValid)
            {
                if ((projectTask.IsClosed()) && await HasIncompleteSubTasks(projectTask.Id))
                {
                    // pass error message back to user client
                    taskViewModelArgs.BannerMessage =
                        "Update Failed: Cannot complete a task that still has open sub tasks.";
                }
                else
                {
                    var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    projectTask.UserId = userId;
                    await _tasksEndpoint.UpdateTask(projectTask);
                    await _commentsEndpoint.UpdateComments(comments);
                    if (!string.IsNullOrWhiteSpace(newComment))
                        await _commentsEndpoint.AddComment(new TaskCommentModel
                            {TaskId = projectTask.Id, Comment = newComment, UserId = userId});

                    if (!string.IsNullOrWhiteSpace(addSubTask))
                    {
                        return RedirectToAction("Create", new { id = addSubTask});
                    }

                    if (projectTask.IsClosed() && projectTask.TaskStatusId != existingTaskStatus)
                    {
                        if (projectTask.HasParentTask())
                        {
                            // return to the parent task
                            return RedirectToAction("Edit", new { id = projectTask.ParentTaskId});
                        }
                        else
                        {
                            // return to the main index page
                            return RedirectToAction("Index");
                        }
                    }
                    
                    return RedirectToAction("Edit", projectTask.Id);
                }
            }

            var model = await TaskViewModel.Create(taskViewModelArgs);

            return View(model);
        }

        public IActionResult Error()
        {
            var model = new BaseViewModel {ProjectId = 0};
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var taskFilterId = _settingsService.GetSetting<int>(nameof(_settingsService.TaskFilterId));
            var projectId = _settingsService.GetSetting<int>(nameof(_settingsService.ProjectId));
            var page = GetPage();

            var project = await _projectsEndpoint.GetProject(projectId) ?? new ProjectModel {Id = 0, Name = "All"};
            var projectTasks = project.Id > 0
                ? await _tasksEndpoint.GetTasks(project.Id)
                : await _tasksEndpoint.GetAllTasks();

            var projects = await _projectsEndpoint.GetAllProjects();
            var taskFilters = _taskFilterService.GetTaskFilters(); 

            var model = new IndexViewModel(projectTasks, taskFilterId, (SettingsModel) _settingsService)
            {
                ProjectId = project.Id,
                Page = page,
                PageSize = _pageSize,
                Projects = projects,
                TaskFilters = taskFilters,
                TaskFilterListItems = GetTaskFilterListItems(taskFilters, taskFilterId),
                ProjectListItems = GetProjectListItems(projects, projectId),
                Project = project,
                ProjectSummaryViewModel = GetProjectSummaryViewModel(project, projectTasks)
            };

            if (Request != null && Request.IsAjaxRequest()) return PartialView("../TaskList/_ProjectTaskList", model);

            return View("../TaskList/Index", model);
        }

        private async Task<bool> HasIncompleteSubTasks(int taskId)
        {
            var subTasks = await _tasksEndpoint.GetSubTasks(taskId);
            return subTasks.Any(subTask => !subTask.IsClosed());
        }
        
        private int GetPage()
        {
            const int defaultPageNo = 1;
            var qsPage = Request.Query["page"];
            return int.TryParse(qsPage, out var pageNo) ? pageNo : defaultPageNo;
        }

        private IEnumerable<SelectListItem> GetProjectListItems(IEnumerable<ProjectModel> projects, int selectedProjectId)
        {
            var projectListItems = new List<SelectListItem>();
            foreach (var project in projects)
            {
                var selectListItem = new SelectListItem()
                    { Text = project.Name, Value = project.Id.ToString(), Selected = project.Id == selectedProjectId };
                projectListItems.Add(selectListItem);
            }

            return projectListItems;
        }
        private IEnumerable<SelectListItem> GetTaskFilterListItems(IEnumerable<TaskFilterModel> taskFilters, int selectedTaskFilterId)
        {
            var taskFilterListItems = new List<SelectListItem>();
            foreach (var taskFilter in taskFilters)
            {
                var selectListItem = new SelectListItem()
                    { Text = taskFilter.Name, Value = taskFilter.Value.ToString(), Selected = taskFilter.Value == selectedTaskFilterId };
                taskFilterListItems.Add(selectListItem);
            }

            return taskFilterListItems;
        }

        private ProjectSummaryViewModel GetProjectSummaryViewModel(ProjectModel currentProject, IList<TaskModel> projectTasks)
        {
            var projectSummary = new ProjectSummaryViewModel();
            projectSummary.ProjectName = currentProject.Name;
            if (projectTasks.Any())
            {
                projectSummary.ProjectCreated =  projectTasks.OrderBy(x => x.Created).First().Created;
                projectSummary.ProjectLastAction =  projectTasks.OrderBy(x => x.Modified).Last().Modified;
                projectSummary.TotalIssues = projectTasks.Count;
                projectSummary.ClosedIssues = projectTasks.Count(x => x.IsClosed());
                projectSummary.OpenIssues = projectTasks.Filtered((int)TaskFilters.Open).Count();
                projectSummary.BacklogIssues = projectTasks.Filtered((int)TaskFilters.Backlog).Count();
                projectSummary.CurrentVersion = projectTasks.OrderBy(x => x.FixedInRelease).Last().FixedInRelease;
            }

            return projectSummary;

        }
    }
}