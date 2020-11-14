using Microsoft.AspNetCore.Http;

namespace Pegasus.Models.Settings
{
    public interface ISettingsModel
    {
        public int CookieExpiryDays { get; set; }
        int PageSize { get; set; }
        bool PaginationEnabled { get; set; }
        public int ProjectId { get; set; }
        public int TaskFilterId { get; set; }

        T GetSetting<T>(HttpRequest request, string settingName);
        void SaveSettings();
    }
}