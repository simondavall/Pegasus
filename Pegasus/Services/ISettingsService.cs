namespace Pegasus.Services
{
    public interface ISettingsService
    {
        public bool AnalyticsCookieEnabled { get; set; }
        public int CommentSortOrder { get; set; }
        public int CookieExpiryDays { get; set; }
        public bool CookiePolicyAccepted { get; set; }
        public bool MarketingCookieEnabled { get; set; }
        int PageSize { get; set; }
        bool PaginationEnabled { get; set; }
        public int ProjectId { get; set; }
        public int TaskFilterId { get; set; }

        T GetSetting<T>(string settingName);
        void SaveSetting(string settingName, string value);
        void SaveSettings();
    }
}