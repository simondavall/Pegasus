using System;

namespace Pegasus.Entities
{
    public class TaskComment
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime Created { get; set; }
        public int TaskId { get; set; }
    }
}
