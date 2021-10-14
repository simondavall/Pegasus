using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        protected readonly Controller Controller;
        protected readonly ITasksEndpoint TasksEndpoint;
        protected readonly IProjectsEndpoint ProjectsEndpoint;
        protected readonly ISettingsService SettingsService;
        protected readonly ICommentsEndpoint CommentsEndpoint;
        protected readonly ITaskFilterService TaskFilterService;

        protected TaskListHelperBase(TaskListHelperArgs args)
        {
            Controller = args.Controller;
            CommentsEndpoint = args.CommentsEndpoint;
            ProjectsEndpoint = args.ProjectsEndpoint;
            TasksEndpoint = args.TasksEndpoint;
            SettingsService = args.SettingsService;
            TaskFilterService = args.TaskFilterService;
        }
        
        internal async Task<TaskViewModel> CreateTaskViewModel(TaskViewModelArgs args)
        {
            var taskProperties = new TaskPropertiesViewModel
            {
                ProjectTask = args.ProjectTask,
                TaskPriorities = new SelectList(await TasksEndpoint.GetAllTaskPriorities(), "Id", "Name", 1),
                TaskStatuses = new SelectList(await TasksEndpoint.GetAllTaskStatuses(), "Id", "Name", 1),
                TaskTypes = new SelectList(await TasksEndpoint.GetAllTaskTypes(), "Id", "Name", 1)
            };

            var model = new TaskViewModel
            {
                BannerMessage = args.BannerMessage,
                Comments =  await GetComments(args.Comments, args.ProjectTask.Id),
                CurrentTaskStatus = args.CurrentStatusId != 0 ? args.CurrentStatusId : args.ProjectTask.TaskStatusId,
                NewComment = args.NewComment,
                ParentTask = await TasksEndpoint.GetTask(args.ProjectTask.ParentTaskId ?? 0),
                Project = args.Project ?? await ProjectsEndpoint.GetProject(args.ProjectTask.ProjectId),
                ProjectId = args.ProjectTask.ProjectId,
                ProjectTask = args.ProjectTask,
                SubTasks = await TasksEndpoint.GetSubTasks(args.ProjectTask.Id),
                TaskProperties = taskProperties
            };

            return model;
        }
        
        private async Task<CommentsViewModel> GetComments(IEnumerable<TaskCommentModel> comments, int projectTaskId)
        {
            comments ??= await CommentsEndpoint.GetComments(projectTaskId);

            return new CommentsViewModel
            {
                Comments = SettingsService.Settings.CommentSortOrder switch
                {
                    (int)CommentSortOrderEnum.DateDescending => comments.OrderByDescending(x => x.Created).ToList(),
                    _ => comments.OrderBy(x => x.Created).ToList()
                }
            };
        }
    }
}