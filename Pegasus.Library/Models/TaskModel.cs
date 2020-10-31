using System;
using System.ComponentModel;

namespace Pegasus.Library.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        [DisplayName("Ref")]
        public string TaskRef { get; set; }
        [DisplayName("Title")]
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayName("Task Type")]
        public int TaskTypeId { get; set; }
        [DisplayName("Task Status")]
        public int TaskStatusId { get; set; }
        public int ProjectId { get; set; }
        [DisplayName("Task Priority")]
        public int TaskPriorityId { get; set; }
        [DisplayName("Fixed In Release")]
        public string FixedInRelease { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
    }

}
