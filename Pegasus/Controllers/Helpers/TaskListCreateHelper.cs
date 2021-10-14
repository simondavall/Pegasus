using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pegasus.Library.Api;
using Pegasus.Library.Models;
using Pegasus.Models.TaskList;
using Pegasus.Services;

namespace Pegasus.Controllers.Helpers
{
    public class TaskListHelperForCreate : TaskListHelperBase
    {
        private readonly Controller _controller;
        private readonly ITasksEndpoint _tasksEndpoint;
        private readonly ISettingsService _settingsService;
        private readonly IProjectsEndpoint _projectsEndpoint;

        public TaskListHelperForCreate(Controller controller, ITasksEndpoint tasksEndpoint, 
            ISettingsService settingsService, IProjectsEndpoint projectsEndpoint, 
            ICommentsEndpoint commentsEndpoint)
            : base(tasksEndpoint, projectsEndpoint, settingsService, commentsEndpoint)
        {
            _controller = controller;
            _tasksEndpoint = tasksEndpoint;
            _settingsService = settingsService;
            _projectsEndpoint = projectsEndpoint;
        }
        
        internal async Task<TaskViewModelArgs> GetTaskViewModelArgs(int? parentTaskId)
        {
            var projectId = _settingsService.GetSetting<int>(nameof(_settingsService.Settings.ProjectId));
            var project = await _projectsEndpoint.GetProject(projectId);
            var taskModel = new TaskModel
            {
                ProjectId = projectId,
                TaskRef = $"{project.ProjectPrefix}-<tbc>",
                ParentTaskId = parentTaskId
            };
            return new TaskViewModelArgs
            {
                ProjectTask = taskModel,
                Project = project
            };
        }

        internal async Task<int> AddProjectTask(TaskModel projectTask)
        {
            projectTask.UserId = _controller.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _tasksEndpoint.AddTask(projectTask);
        }
        
        internal async Task<(bool isValid, IActionResult actionResult)> IsDataValid(TaskModel projectTask)
        {
            if (!_controller.ModelState.IsValid)
            {
                var model = await CreateTaskViewModel(new TaskViewModelArgs
                {
                    ProjectTask = projectTask
                });

                return (false, _controller.View(model));
            }
            
            return (true, null);
        }
    }
}