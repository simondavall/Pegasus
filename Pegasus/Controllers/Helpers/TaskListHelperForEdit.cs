using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pegasus.Extensions;
using Pegasus.Library.Models;
using Pegasus.Library.Services.Resources;
using Pegasus.Models.TaskList;

namespace Pegasus.Controllers.Helpers
{
    public class TaskListHelperForEdit : TaskListHelperBase
    {
        public TaskListHelperForEdit(TaskListHelperArgs args) : base(args)
        { }

        internal async Task<(bool isValid, IActionResult)> IsDataValid(TaskModel projectTask, TaskViewModelArgs taskViewModelArgs)
        {
            if (!Controller.ModelState.IsValid)
            {
                var model = await CreateTaskViewModel(taskViewModelArgs);
                return (false, Controller.View(model));
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
                return (false, Controller.View(model1));
            }

            return (true, null);
        }

        private async Task<bool> HasIncompleteSubTasks(int taskId)
        {
            var subTasks = await TasksEndpoint.GetSubTasks(taskId);
            return subTasks.Any(subTask => !subTask.IsClosed());
        }

        internal void UpdateSettingsWithCurrentProject(TaskModel projectTask)
        {
            if (projectTask != null)
            {
                SettingsService.Settings.ProjectId = projectTask.ProjectId;
                SettingsService.SaveSettings();
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
            await CommentsEndpoint.UpdateComments(comments.ToList());
            if (!string.IsNullOrWhiteSpace(newComment))
                await CommentsEndpoint.AddComment(new TaskCommentModel
                    {TaskId = projectTask.Id, Comment = newComment, UserId = projectTask.UserId});
        }

        internal async Task UpdateProjectTaskData(TaskModel projectTask, string newComment, IList<TaskCommentModel> comments)
        {
            projectTask.UserId = Controller.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await TasksEndpoint.UpdateTask(projectTask);
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
                    
            return Controller.RedirectToAction("Edit", "TaskList", new { id = (int?)projectTask.Id} );
        }

        private (bool isSubTaskAdded, IActionResult actionResult) IsNewSubTaskToBeAdded(string addSubTask)
        {
            if (!string.IsNullOrWhiteSpace(addSubTask))
            {
                return (true, Controller.RedirectToAction("Create", "TaskList", new { id = addSubTask}));
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
            return Controller.RedirectToAction("Edit", "TaskList", new { id = projectTask.ParentTaskId });
        } 
        
        private IActionResult ReturnToIndexPage()
        {
            return Controller.RedirectToAction("Index", "TaskList");
        } 
    }
}