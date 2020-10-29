using System;

namespace PegasusApi.Models
{
    public class TaskStatusHistoryModel
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int TaskStatusId { get; set; }
        public DateTime Created { get; set; }
    }
}
