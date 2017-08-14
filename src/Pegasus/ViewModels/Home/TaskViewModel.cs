using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pegasus.Entities;
using Pegasus.Services;

namespace Pegasus.ViewModels.Home
{
    public class TaskViewModel : BaseViewModel
    {
        public ProjectTask ProjectTask { get; set; }
        public IEnumerable<SelectListItem> TaskStatuses { get; set; }
        public IEnumerable<SelectListItem> TaskTypes { get; set; }
        public IEnumerable<SelectListItem> TaskPriorities { get; set; }
        public Project Project { get; set; }
        public string FixedInRelease { get; set; }
        public string Action { get; set; }
        public string ButtonText { get; set; }

        public int ExistingTaskStatus { get; set; }

        public static TaskViewModel Create(IPegasusData db, ProjectTask projectTask, Project project)
        {
            return Create(db, projectTask, project, projectTask.TaskStatusId);
        }

        public static TaskViewModel Create(IPegasusData db, ProjectTask projectTask, Project project, int existingTaskStatus)
        {
            return
                new TaskViewModel
                {
                    ProjectId = projectTask.ProjectId,
                    TaskTypes = new SelectList(db.GetAllTaskTypes(), "Id", "Name", 1),
                    TaskStatuses = new SelectList(db.GetAllTaskStatuses(), "Id", "Name", 1),
                    TaskPriorities = new SelectList(db.GetAllTaskPriorities(), "Id", "Name", 1),
                    ProjectTask = projectTask,
                    Project = project,
                    ExistingTaskStatus = existingTaskStatus
                };
        }
    }
}
