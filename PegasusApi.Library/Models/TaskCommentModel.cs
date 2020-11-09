using System;

namespace PegasusApi.Library.Models
{
    public class TaskCommentModel
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Comment { get; set; }
        public bool IsDeleted { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
    }
}
