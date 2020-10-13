using System.Collections.Generic;
using System.Linq;

namespace Pegasus.Entities.Sorters.ProjectTask
{
    public static class ProjectTaskSortExt
    {
        public static IOrderedEnumerable<ProjectTaskExt> Sorted(this IEnumerable<ProjectTaskExt> projectTaskExts, ISorter sorter)
        {
            return sorter.Sort(projectTaskExts);
        }
    }
}
