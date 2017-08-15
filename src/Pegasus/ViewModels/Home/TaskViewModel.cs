using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pegasus.Entities;

namespace Pegasus.ViewModels.Home
{
    public class TaskViewModel : BaseViewModel
    {
        public ProjectTask ProjectTask { get; set; }
        public IEnumerable<SelectListItem> TaskStatuses { get; set; }
        public IEnumerable<SelectListItem> TaskTypes { get; set; }
        public IEnumerable<SelectListItem> TaskPriorities { get; set; }
        public Project Project { get; set; }
        public IEnumerable<TaskComment> Comments { get; set; }
        [Display(Name="Add Comment")]
        [DataType(DataType.MultilineText)]
        public string NewComment { get; set; }

        public int ExistingTaskStatus { get; set; }

        public static TaskViewModel Create(TaskViewModelArgs args)
        {
            return
                new TaskViewModel
                {
                    ProjectId = args.ProjectTask.ProjectId,
                    TaskTypes = new SelectList(args.PegasusData.GetAllTaskTypes(), "Id", "Name", 1),
                    TaskStatuses = new SelectList(args.PegasusData.GetAllTaskStatuses(), "Id", "Name", 1),
                    TaskPriorities = new SelectList(args.PegasusData.GetAllTaskPriorities(), "Id", "Name", 1),
                    Comments = args.Comments ?? args.PegasusData.GetComments(args.ProjectTask.Id),
                    ProjectTask = args.ProjectTask,
                    Project = args.Project ?? args.PegasusData.GetProject(args.ProjectTask.ProjectId),
                    ExistingTaskStatus = args.ExistingStatusId != 0 ? args.ExistingStatusId : args.ProjectTask.TaskStatusId,
                    NewComment = args.NewComment
                };
        }
    }
}
