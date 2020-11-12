using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Pegasus.Models.Settings
{
    public class SettingsModel : ISettingsModel
    {
        private readonly IConfiguration _configuration;

        public SettingsModel()
        {
        }

        public SettingsModel(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _configuration = configuration;
            var cookieSettings = LoadSettingsFromCookies(httpContextAccessor);
            InitializeSettings(cookieSettings);
        }

        [DisplayName("Page Size")]
        public int PageSize { get; set; }
        [DisplayName("Disable Pagination")] 
        public bool PaginationDisabled { get; set; }

        private Dictionary<string, object> LoadSettingsFromCookies(IHttpContextAccessor httpContextAccessor)
        {
            var settingsJson = httpContextAccessor.HttpContext.Request.Cookies["userSettings"];
            if (!string.IsNullOrWhiteSpace(settingsJson))
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(settingsJson);
            }

            return null;
        }

        private void InitializeSettings(IReadOnlyDictionary<string, object> propertiesFromCookie)
        {
            //I chose to treat each property individually rather than just serialize/deserialize the SettingsModel
            //as a whole, as this would lead to issues when adding a new setting later on. It would be difficult to 
            //workout whether the new setting is already stored in the cookie or not, as default values returned during
            //deserialization could be misleading.
            // E.g Say a new bool property is added, ShowWidget. This would not currently exist in the settings stored
            //in the cookie. But deserialize would return the default value 'false'. But it might be desirable to set
            //the new field to 'true' by default in appsettings, which would then be carried forward to the cookie on
            //the next save.
            //So I currently see no alternative than to save each property to the cookie individually, so that missing
            //'new' properties can be read from appsettings.json

            //PageSize
            if (propertiesFromCookie.TryGetValue("PageSize", out var pageSizeObj))
            {
                if (int.TryParse(pageSizeObj.ToString(), out int pageSize))
                {
                    PageSize = pageSize;
                }
            }
            else
            {
                PageSize = _configuration.GetValue<int>("Pagination:PageSize");
            }

            //Disable Pagination
            if (propertiesFromCookie.TryGetValue("PaginationDisabled", out var paginationDisabledObj))
            {
                if (bool.TryParse(paginationDisabledObj.ToString(), out bool paginationDisabled))
                {
                    PaginationDisabled = paginationDisabled;
                }
            }
            else
            {
                PaginationDisabled = _configuration.GetValue<bool>("Pagination:PaginationDisabled");
            }
        }
    }
}
