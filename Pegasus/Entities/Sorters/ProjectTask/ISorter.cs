using System.Collections.Generic;
using System.Linq;
using Pegasus.Library.Models;

namespace Pegasus.Entities.Sorters.ProjectTask
{
    public interface ISorter
    {
        IOrderedEnumerable<TaskModel> Sort(IEnumerable<TaskModel> projectTasks);
    }
}
