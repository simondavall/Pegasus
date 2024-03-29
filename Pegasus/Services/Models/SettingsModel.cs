﻿using System.ComponentModel;

namespace Pegasus.Services.Models
{
    public class SettingsModel
    {
        [DisplayName("Comment Order")] 
        public int CommentSortOrder { get; set; }
        public int CookieExpiryDays { get; set; }
        [DisplayName("Page Size")]
        public int PageSize { get; set; }
        [DisplayName("Pagination Enabled")] 
        public bool PaginationEnabled { get; set; }
        public int ProjectId { get; set; }
        public int TaskFilterId { get; set; }

        // cookie related settings
        public bool AnalyticsCookieEnabled { get; set; }
        public bool MarketingCookieEnabled { get; set; }
        public bool CookiePolicyAccepted { get; set; }
    }
}
