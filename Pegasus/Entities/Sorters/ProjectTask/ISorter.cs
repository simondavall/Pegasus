using System.Collections.Generic;
using System.Linq;

namespace Pegasus.Entities.Sorters.ProjectTask
{
    public interface ISorter
    {
        IOrderedEnumerable<ProjectTaskExt> Sort(IEnumerable<ProjectTaskExt> projectTaskExts);
    }
}
