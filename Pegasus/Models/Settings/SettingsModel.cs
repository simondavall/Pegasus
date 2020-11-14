using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using Pegasus.Domain;

namespace Pegasus.Models.Settings
{
    public class SettingsModel : ISettingsModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private const string Position = "Pagination";
        private readonly Cookies _cookies;

        public SettingsModel()
        {
        }

        public SettingsModel(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            configuration.GetSection(Position).Bind(this);
            var cookieSettings = LoadSettingsFromCookies(httpContextAccessor);
            InitializeSettings(cookieSettings);
            _cookies = new Cookies(configuration);
        }

        [DisplayName("Page Size")]
        public int PageSize { get; set; }
        [DisplayName("Pagination Enabled")] 
        public bool PaginationEnabled { get; set; }

        //public int TaskFilterId { get; set; }
        //public int ProjectId { get; set; }


        private Dictionary<string, object> LoadSettingsFromCookies(IHttpContextAccessor httpContextAccessor)
        {
            var settingsJson = httpContextAccessor.HttpContext.Request.Cookies["userSettings"];
            if (!string.IsNullOrWhiteSpace(settingsJson))
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(settingsJson);
            }

            return new Dictionary<string, object>();
        }

        public void SaveSettings()
        {
            var propertyValues = new Dictionary<string, object>();
            var properties = typeof(SettingsModel).GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(this, null);
                propertyValues.Add(property.Name, value);
            }

            var cookieData =  JsonSerializer.Serialize(propertyValues);
            var expiryDays = GetUserSettingsExpiryDays();
            _cookies.WriteCookie(_httpContextAccessor.HttpContext.Response, "userSettings", cookieData, expiryDays);
        }

        private int GetUserSettingsExpiryDays()
        {
            try
            {
                return _configuration.GetValue<int>("Cookies:UserSettings:ExpiryDays");
            }
            catch (InvalidOperationException ex)
            {
                //TODO Handle this exception better
                return 0;
            }
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
                if (propertiesFromCookie.TryGetValue(property.Name, out var value))
                {
                    var convertedValue = Convert.ChangeType(value?.ToString(), property.PropertyType);
                    property.SetValue(this, convertedValue);
                };
            }
        }
    }
}
