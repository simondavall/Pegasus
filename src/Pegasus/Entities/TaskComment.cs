using System;
using System.ComponentModel.DataAnnotations;

namespace Pegasus.Entities
{
    public class TaskComment
    {
        public int Id { get; set; }
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }
        public DateTime Created { get; set; }
        public int TaskId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
