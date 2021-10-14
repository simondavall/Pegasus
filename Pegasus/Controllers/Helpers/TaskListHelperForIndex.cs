using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pegasus.Library.Api;
using Pegasus.Library.Models;
using Pegasus.Models.TaskList;
using Pegasus.Services;

namespace Pegasus.Controllers.Helpers
{
    public class TaskListHelperForIndex : TaskListHelperBase
    {
        private readonly Controller _controller;
        private readonly int _pageSize;
        private readonly IProjectsEndpoint _projectsEndpoint;
        private readonly ISettingsService _settingsService;
        private readonly ITaskFilterService _taskFilterService;
        private readonly ITasksEndpoint _tasksEndpoint;
        
        public TaskListHelperForIndex(Controller controller, ITaskFilterService taskFilterService,
            IProjectsEndpoint projectsEndpoint, ITasksEndpoint tasksEndpoint,
            ICommentsEndpoint commentsEndpoint, ISettingsService settingsService)
            : base(tasksEndpoint, projectsEndpoint, settingsService, commentsEndpoint)
        {
            _controller = controller;
            _taskFilterService = taskFilterService;
            _projectsEndpoint = projectsEndpoint;
            _tasksEndpoint = tasksEndpoint;
            _settingsService = settingsService;
            _pageSize = settingsService.Settings.PageSize;
        }
        
        internal async Task<IndexViewModel> GetIndexViewModel()
        {
            var taskFilterId = _settingsService.GetSetting<int>(nameof(_settingsService.Settings.TaskFilterId));
            var projectId = _settingsService.GetSetting<int>(nameof(_settingsService.Settings.ProjectId));
            var page = GetPage();

            var project = await _projectsEndpoint.GetProject(projectId) ?? new ProjectModel {Id = 0, Name = "All"};
            var projectTasks = project.Id > 0
                ? await _tasksEndpoint.GetTasks(project.Id)
                : await _tasksEndpoint.GetAllTasks();

            return new IndexViewModel(projectTasks, taskFilterId, _settingsService.Settings)
            {
                ProjectId = project.Id,
                Page = page,
                PageSize = _pageSize,
                Projects = await _projectsEndpoint.GetAllProjects(),
                TaskFilters = _taskFilterService.GetTaskFilters(),
                Project = project
            };
        }
        
        private int GetPage()
        {
            const int defaultPageNo = 1;
            var qsPage = _controller.Request.Query["page"];
            return int.TryParse(qsPage, out var pageNo) ? pageNo : defaultPageNo;
        }
    }
}