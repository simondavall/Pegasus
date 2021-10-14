using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pegasus.Extensions;
using Pegasus.Library.Api;
using Pegasus.Library.Models;
using Pegasus.Library.Services.Resources;
using Pegasus.Models.TaskList;
using Pegasus.Services;

namespace Pegasus.Controllers.Helpers
{
    public class TaskListHelperForEdit : TaskListHelperBase
    {
        private readonly Controller _controller;
        private readonly ICommentsEndpoint _commentsEndpoint;
        private readonly ISettingsService _settingsService;
        private readonly ITasksEndpoint _tasksEndpoint;
        
        public TaskListHelperForEdit(Controller controller,
            IProjectsEndpoint projectsEndpoint, ITasksEndpoint tasksEndpoint,
            ICommentsEndpoint commentsEndpoint, ISettingsService settingsService)
            : base(tasksEndpoint, projectsEndpoint, settingsService, commentsEndpoint)
        {
            _controller = controller;
            _tasksEndpoint = tasksEndpoint;
            _commentsEndpoint = commentsEndpoint;
            _settingsService = settingsService;
        }

        internal async Task<(bool isValid, IActionResult)> IsDataValid(TaskModel projectTask, TaskViewModelArgs taskViewModelArgs)
        {
            if (!_controller.ModelState.IsValid)
            {
                var model = await CreateTaskViewModel(taskViewModelArgs);
                return (false, _controller.View(model));
            }

            var (isValidClose, actionResult) = await IsClosingTaskWithOpenSubTasks(projectTask, taskViewModelArgs);
            if (!isValidClose)
                return (false, actionResult);

            return (true, null);
        }
        
        private async Task<(bool isValidClose, IActionResult actionResult)> IsClosingTaskWithOpenSubTasks(TaskModel projectTask, TaskViewModelArgs taskViewModelArgs)
        {
            if (projectTask.IsClosed() && await HasIncompleteSubTasks(projectTask.Id))
            {
                taskViewModelArgs.BannerMessage = Resources.ControllerStrings.TaskListController.CannotCloseWithOpenSubTasks;

                var model1 = await CreateTaskViewModel(taskViewModelArgs);
                return (false, _controller.View(model1));
            }

            return (true, null);
        }

        private async Task<bool> HasIncompleteSubTasks(int taskId)
        {
            var subTasks = await _tasksEndpoint.GetSubTasks(taskId);
            return subTasks.Any(subTask => !subTask.IsClosed());
        }

        internal void UpdateSettingsWithCurrentProject(TaskModel projectTask)
        {
            if (projectTask != null)
            {
                _settingsService.Settings.ProjectId = projectTask.ProjectId;
                _settingsService.SaveSettings();
            }
        }

        internal async Task<TaskViewModel> CreateModel(TaskModel projectTask)
        {
            return await CreateTaskViewModel(new TaskViewModelArgs
            {
                ProjectTask = projectTask
            });
        }
        
        private async Task UpdateComments(string newComment, IList<TaskCommentModel> comments, TaskModel projectTask)
        {
            await _commentsEndpoint.UpdateComments(comments.ToList());
            if (!string.IsNullOrWhiteSpace(newComment))
                await _commentsEndpoint.AddComment(new TaskCommentModel
                    {TaskId = projectTask.Id, Comment = newComment, UserId = projectTask.UserId});
        }

        internal async Task UpdateProjectTaskData(TaskModel projectTask, string newComment, IList<TaskCommentModel> comments)
        {
            projectTask.UserId = _controller.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _tasksEndpoint.UpdateTask(projectTask);
            await UpdateComments(newComment, comments, projectTask);
        }
        
        internal IActionResult RedirectFromEdit(TaskModel projectTask, string addSubTask, int currentTaskStatus)
        {
            var (isSubTaskAdded, subTaskActionResult) = IsNewSubTaskToBeAdded(addSubTask);
            if (isSubTaskAdded)
                return subTaskActionResult;
            
            var (isClosed, closedActionResult) = IsTaskClosed(projectTask, currentTaskStatus);
            if (isClosed)
                return closedActionResult;
                    
            return _controller.RedirectToAction("Edit", "TaskList", new { id = (int?)projectTask.Id} );
        }

        private (bool isSubTaskAdded, IActionResult actionResult) IsNewSubTaskToBeAdded(string addSubTask)
        {
            if (!string.IsNullOrWhiteSpace(addSubTask))
            {
                return (true, _controller.RedirectToAction("Create", "TaskList", new { id = addSubTask}));
            }

            return (false, null);
        }

        private (bool isClosed, IActionResult actionResult) IsTaskClosed(TaskModel projectTask, int currentTaskStatus)
        {
            if (projectTask.IsClosed() && projectTask.TaskStatusId != currentTaskStatus)
            {
                if (projectTask.HasParentTask())
                {
                    return (true, ReturnToParentTask(projectTask));
                }
                
                return (true, ReturnToIndexPage());
            }

            return (false, null);
        }

        private IActionResult ReturnToParentTask(TaskModel projectTask)
        {
            return _controller.RedirectToAction("Edit", "TaskList", new { id = projectTask.ParentTaskId });
        } 
        
        private IActionResult ReturnToIndexPage()
        {
            return _controller.RedirectToAction("Index", "TaskList");
        } 
    }
}