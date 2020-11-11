using System.ComponentModel;

namespace Pegasus.Models.Settings
{
    public class SettingsModel : ISettingsModel
    {
        public string Title { get; set; } = "Settings";
        [DisplayName("Page Size")]
        public int PageSize { get; set; }
        [DisplayName("Show Pagination")] 
        public bool ShowPagination { get; set; } = true;
    }
}
