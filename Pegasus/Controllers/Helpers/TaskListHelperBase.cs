using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pegasus.Entities.Enumerations;
using Pegasus.Library.Api;
using Pegasus.Library.Models;
using Pegasus.Models.TaskList;
using Pegasus.Services;

namespace Pegasus.Controllers.Helpers
{
    public class TaskListHelperBase
    {
        private readonly ITasksEndpoint _tasksEndpoint;
        private readonly IProjectsEndpoint _projectsEndpoint;
        private readonly ISettingsService _settingsService;
        private readonly ICommentsEndpoint _commentsEndpoint;

        protected TaskListHelperBase(ITasksEndpoint tasksEndpoint, IProjectsEndpoint projectsEndpoint, ISettingsService settingsService, ICommentsEndpoint commentsEndpoint)
        {
            _tasksEndpoint = tasksEndpoint;
            _projectsEndpoint = projectsEndpoint;
            _settingsService = settingsService;
            _commentsEndpoint = commentsEndpoint;
        }
        
        internal async Task<TaskViewModel> CreateTaskViewModel(TaskViewModelArgs args)
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
    }
}