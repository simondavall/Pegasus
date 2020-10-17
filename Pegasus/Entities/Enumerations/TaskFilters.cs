using System.ComponentModel.DataAnnotations;

namespace Pegasus.Entities.Enumerations
{
    public enum TaskFilters
    {
        [Display(Name = "All Tasks")]
        All = 0,
        [Display(Name = "Open Tasks")]
        Open,
        [Display(Name = "High Priority")]
        HighPriority,
        [Display(Name = "Obsolete Tasks")]
        Obsolete
    }
}
