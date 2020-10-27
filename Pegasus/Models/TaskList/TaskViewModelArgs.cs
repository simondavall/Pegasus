using System.Collections.Generic;
using Pegasus.Library.Api;
using Pegasus.Library.Models;

namespace Pegasus.Models.TaskList
{
    public class TaskViewModelArgs
    {
        public ITasksEndpoint TasksEndpoint { get; set; }
        public IProjectsEndpoint ProjectsEndpoint { get; set; }
        public ICommentsEndpoint CommentsEndpoint { get; set; }
        public TaskModel ProjectTask { get; set; }
        public ProjectModel Project { get; set; }
        public int ExistingStatusId { get; set; } = 0;
        public string NewComment { get; set; } = "";
        public IEnumerable<TaskCommentModel> Comments { get; set; }
    }
}
