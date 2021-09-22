namespace Pegasus.Services
{
    public interface ISettingsService
    {
        public int CommentSortOrder { get; set; }
        public int CookieExpiryDays { get; set; }
        int PageSize { get; set; }
        bool PaginationEnabled { get; set; }
        public int ProjectId { get; set; }
        public int TaskFilterId { get; set; }

        T GetSetting<T>(string settingName);
        void SaveSettings();
    }
}