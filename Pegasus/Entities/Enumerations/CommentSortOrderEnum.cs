using System.ComponentModel.DataAnnotations;

namespace Pegasus.Entities.Enumerations
{
    public enum CommentSortOrderEnum
    {
        [Display(Name = "Date Ascending")]
        DateAscending,
        [Display(Name = "Date Descending")]
        DateDescending
    }
}
