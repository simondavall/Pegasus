using System.Collections.Generic;
using Pegasus.Entities;


namespace Pegasus.ViewModels.Home
{
    public class DetailsViewModel
    {
        public ProjectTaskExt ProjectTask { get; set; }
        public IEnumerable<Project> Projects { get; set; }
        public IEnumerable<TaskStatus> TaskStatuses { get; set; }
        public IEnumerable<TaskType> TaskTypes { get; set; }
    }

    public class ProjectTaskExt : ProjectTask
    {
        public ProjectTaskExt(ProjectTask task)
        {
            Id = task.Id;
            ProjectId = task.ProjectId;
            Created = task.Created;
            Modified = task.Modified;
            Name = task.Name;
            Description = task.Description;
            TaskRef = task.TaskRef;
            TaskStatusId = task.TaskStatusId;
            TaskTypeId = task.TaskTypeId;
        }

        public IEnumerable<TaskStatusHistory> StatusHistory { get; set; }
        public IEnumerable<TaskComment> Comments { get; set; }
        public Project Project { get; set; }
        public TaskType TaskType { get; set; }
        public TaskStatus TaskStatus { get; set; }
    }
}
