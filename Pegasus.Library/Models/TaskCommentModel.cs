using System;

namespace Pegasus.Library.Models
{
    public class TaskCommentModel
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime Created { get; set; }
        public int TaskId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
