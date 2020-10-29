using System;

namespace PegasusApi.Library.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string TaskRef { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TaskTypeId { get; set; }
        public int TaskStatusId { get; set; }
        public int ProjectId { get; set; }
        public int TaskPriorityId { get; set; }
        public string FixedInRelease { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
    }
}
