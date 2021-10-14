using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pegasus.Library.Models;
using Pegasus.Models.TaskList;

namespace Pegasus.Controllers.Helpers
{
    public class TaskListHelperForCreate : TaskListHelperBase
    {
        public TaskListHelperForCreate(TaskListHelperArgs args) : base(args)
        { }
        
        internal async Task<TaskViewModelArgs> GetTaskViewModelArgs(int? parentTaskId)
        {
            var projectId = SettingsService.GetSetting<int>(nameof(SettingsService.Settings.ProjectId));
            var project = await ProjectsEndpoint.GetProject(projectId);
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
            projectTask.UserId = Controller.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await TasksEndpoint.AddTask(projectTask);
        }
        
        internal async Task<(bool isValid, IActionResult actionResult)> IsDataValid(TaskModel projectTask)
        {
            if (!Controller.ModelState.IsValid)
            {
                var model = await CreateTaskViewModel(new TaskViewModelArgs
                {
                    ProjectTask = projectTask
                });

                return (false, Controller.View(model));
            }
            
            return (true, null);
        }
    }
}