using System.Collections.Generic;
using System.Linq;
using Pegasus.Entities;
using Pegasus.Entities.Enumerations;

namespace Pegasus.Models.TaskList
{
    public static class IndexViewModelExtensions
    {
        private static IEnumerable<ProjectTaskExt> IsNotObsolete(this IEnumerable<ProjectTaskExt> projectTasks)
        {
            return projectTasks.Where(pt => pt.TaskStatusId != (int)TaskStatusEnum.Obsolete);
        }
        private static IEnumerable<ProjectTaskExt> IsObsolete(this IEnumerable<ProjectTaskExt> projectTasks)
        {
            return projectTasks.Where(pt => pt.TaskStatusId == (int)TaskStatusEnum.Obsolete);
        }

        internal static IEnumerable<ProjectTaskExt> Filtered(this IEnumerable<ProjectTaskExt> projectTasks, int taskFilterId)
        {
            return (TaskFilters)taskFilterId switch
            {
                TaskFilters.Open => projectTasks.Where(pt => !pt.IsClosed).IsNotObsolete(),
                TaskFilters.HighPriority => projectTasks.Where(pt => pt.TaskPriorityId > 3).IsNotObsolete(),
                TaskFilters.Obsolete => projectTasks.IsObsolete(),
                _ => projectTasks.IsNotObsolete()
            };
        }
    }
}
