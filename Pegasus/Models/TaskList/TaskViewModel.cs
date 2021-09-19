using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pegasus.Library.Models;

namespace Pegasus.Models.TaskList
{
    public class TaskViewModel : BaseViewModel
    {
        public string BannerMessage { get; set; }
        public IEnumerable<TaskCommentModel> Comments { get; set; }
        public int ExistingTaskStatus { get; set; }
        [Display(Name="Add Comment")]
        [DataType(DataType.MultilineText)]
        public string NewComment { get; set; }
        public TaskModel ParentTask { get; set; }
        public ProjectModel Project { get; set; }
        public TaskModel ProjectTask { get; set; }
        public IEnumerable<TaskModel> SubTasks { get; set; }
        public TaskPropertiesViewModel TaskProperties { get; set; }

        public static async Task<TaskViewModel> Create(TaskViewModelArgs args)
        {
            var taskProperties = new TaskPropertiesViewModel()
            {
                ProjectTask = args.ProjectTask,
                TaskPriorities = new SelectList(await args.TasksEndpoint.GetAllTaskPriorities(), "Id", "Name", 1),
                TaskStatuses = new SelectList(await args.TasksEndpoint.GetAllTaskStatuses(), "Id", "Name", 1),
                TaskTypes = new SelectList(await args.TasksEndpoint.GetAllTaskTypes(), "Id", "Name", 1)
            };

            var model = new TaskViewModel
            {
                BannerMessage = args.BannerMessage,
                Comments = args.Comments ?? await args.CommentsEndpoint.GetComments(args.ProjectTask.Id),
                ExistingTaskStatus = args.ExistingStatusId != 0 ? args.ExistingStatusId : args.ProjectTask.TaskStatusId,
                NewComment = args.NewComment,
                ParentTask = await args.TasksEndpoint.GetTask(args.ProjectTask.ParentTaskId ?? 0),
                Project = args.Project ?? await args.ProjectsEndpoint.GetProject(args.ProjectTask.ProjectId),
                ProjectId = args.ProjectTask.ProjectId,
                ProjectTask = args.ProjectTask,
                SubTasks = await args.TasksEndpoint.GetSubTasks(args.ProjectTask.Id),
                TaskProperties = taskProperties
            };

            return model;
        }
    }
}
