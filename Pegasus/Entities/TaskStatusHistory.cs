using System;

namespace Pegasus.Entities
{
    public class TaskStatusHistory
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int TaskStatusId { get; set; }
        public DateTime Created { get; set; }
    }
}
