using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pegasus.Library.Models;

namespace Pegasus.Models.TaskList
{
    public class TaskViewModel : BaseViewModel
    {
        public TaskModel ProjectTask { get; set; }
        public IEnumerable<SelectListItem> TaskStatuses { get; set; }
        public IEnumerable<SelectListItem> TaskTypes { get; set; }
        public IEnumerable<SelectListItem> TaskPriorities { get; set; }
        public ProjectModel Project { get; set; }
        public IEnumerable<TaskCommentModel> Comments { get; set; }
        [Display(Name="Add Comment")]
        [DataType(DataType.MultilineText)]
        public string NewComment { get; set; }

        public int ExistingTaskStatus { get; set; }

        public static async Task<TaskViewModel> Create(TaskViewModelArgs args)
        {
            var model = new TaskViewModel
            {
                ProjectId = args.ProjectTask.ProjectId,
                TaskTypes = new SelectList(await args.TasksEndpoint.GetAllTaskTypes(), "Id", "Name", 1),
                TaskStatuses = new SelectList(await args.TasksEndpoint.GetAllTaskStatuses(), "Id", "Name", 1),
                TaskPriorities = new SelectList(await args.TasksEndpoint.GetAllTaskPriorities(), "Id", "Name", 1),
                Comments = args.Comments ?? await args.CommentsEndpoint.GetComments(args.ProjectTask.Id),
                ProjectTask = args.ProjectTask,
                Project = args.Project ?? await args.ProjectsEndpoint.GetProject(args.ProjectTask.ProjectId),
                ExistingTaskStatus = args.ExistingStatusId != 0 ? args.ExistingStatusId : args.ProjectTask.TaskStatusId,
                NewComment = args.NewComment
            };

            return model;
        }
    }
}
