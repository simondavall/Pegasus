using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pegasus.Library.Models;

namespace Pegasus.Models.TaskList
{
    public class TaskViewModel : BaseViewModel
    {
        public string BannerMessage { get; set; }
        public CommentsViewModel Comments { get; set; }
        public int ExistingTaskStatus { get; set; }
        [Display(Name="Add Comment")]
        [DataType(DataType.MultilineText)]
        public string NewComment { get; set; }
        public TaskModel ParentTask { get; set; }
        public ProjectModel Project { get; set; }
        public TaskModel ProjectTask { get; set; }
        public IEnumerable<TaskModel> SubTasks { get; set; }
        public TaskPropertiesViewModel TaskProperties { get; set; }
    }
}
