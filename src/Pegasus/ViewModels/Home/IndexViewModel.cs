using System.Collections.Generic;
using Pegasus.Entities;
using TaskStatus = Pegasus.Entities.TaskStatus;

namespace Pegasus.ViewModels.Home
{
    public class IndexViewModel
    {
        public IEnumerable<ProjectTask> ProjectTasks { get; set; }
        public IEnumerable<Project> Projects { get; set; }
        public IEnumerable<TaskStatus> TaskStatuses { get; set; }
        public IEnumerable<TaskType> TaskTypes { get; set; }
    }
}
