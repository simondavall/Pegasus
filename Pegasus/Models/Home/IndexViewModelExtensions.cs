using System.Collections.Generic;
using System.Linq;
using Pegasus.Entities;
using Pegasus.Entities.Enumerations;

namespace Pegasus.Models.Home
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

        internal static IEnumerable<ProjectTaskExt> FilteredProjects(this IEnumerable<ProjectTaskExt> projectTasks, int taskFilterId)
        {
            switch (taskFilterId)
            {
                case 1:
                    return projectTasks.Where(pt => !pt.IsClosed).IsNotObsolete();
                case 2:
                    return projectTasks.Where(pt => pt.TaskPriorityId > 3).IsNotObsolete();
                case 3:
                    return projectTasks.IsObsolete();
                default:
                    return projectTasks.IsNotObsolete();
            }
        }
    }
}
