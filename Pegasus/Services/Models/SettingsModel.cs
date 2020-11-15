using System.ComponentModel;

namespace Pegasus.Services.Models
{
    public class SettingsModel
    {
        public int CookieExpiryDays { get; set; }
        [DisplayName("Page Size")]
        public int PageSize { get; set; }
        [DisplayName("Pagination Enabled")] 
        public bool PaginationEnabled { get; set; }
        public int ProjectId { get; set; }
        public int TaskFilterId { get; set; }

    }
}
