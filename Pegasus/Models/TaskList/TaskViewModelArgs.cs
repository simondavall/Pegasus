using System.Collections.Generic;
using Pegasus.Library.Models;

namespace Pegasus.Models.TaskList
{
    public class TaskViewModelArgs
    {
        public string BannerMessage { get; set; }
        public IEnumerable<TaskCommentModel> Comments { get; set; }
        public int CurrentStatusId { get; set; } = 0;
        public string NewComment { get; set; } = "";
        public ProjectModel Project { get; set; }
        public TaskModel ProjectTask { get; set; }
    }
}
