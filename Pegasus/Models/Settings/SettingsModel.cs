using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;

namespace Pegasus.Models.Settings
{
    public class SettingsModel : ISettingsModel
    {
        private const string Position = "Pagination";

        public SettingsModel()
        {
        }

        public SettingsModel(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            configuration.GetSection(Position).Bind(this);
            var cookieSettings = LoadSettingsFromCookies(httpContextAccessor);
            InitializeSettings(cookieSettings);
        }

        [DisplayName("Page Size")]
        public int PageSize { get; set; }
        [DisplayName("Pagination Enabled")] 
        public bool PaginationEnabled { get; set; }

        private Dictionary<string, object> LoadSettingsFromCookies(IHttpContextAccessor httpContextAccessor)
        {
            var settingsJson = httpContextAccessor.HttpContext.Request.Cookies["userSettings"];
            if (!string.IsNullOrWhiteSpace(settingsJson))
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(settingsJson);
            }

            return new Dictionary<string, object>();
        }

        private void InitializeSettings(IReadOnlyDictionary<string, object> propertiesFromCookie)
        {
            //I chose to treat each property individually in a dictionary, rather than just serialize/deserialize the SettingsModel
            //as a whole, as this would lead to an issue when adding a new setting later on. It would be impossible to 
            //workout whether the new setting is already stored in the cookie or not, as default values returned during
            //deserialization could be misleading.
            // E.g Say a new bool property is added, ShowWidget, with a config setting of 'true'. This setting would
            // not currently exist in the cookie data and would default to 'false' upon retrieval from the cook and overwrite the
            // value set by config.

            foreach (var property in typeof(SettingsModel).GetProperties())
            {
                propertiesFromCookie.TryGetValue(property.Name, out var value);
                var convertedValue = Convert.ChangeType(value?.ToString(), property.PropertyType);
                property.SetValue(this, convertedValue);
            }
        }
    }
}
