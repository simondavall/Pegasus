using System;

namespace Pegasus.Library.Models
{
    public class TaskStatusHistoryModel
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int TaskStatusId { get; set; }
        public string UserId { get; set; }
        public DateTime Created { get; set; }
    }
}
