using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Pegasus.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;

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
            _cookies = new Cookies(this);
            InitializeSettings();
        }

        public int CookieExpiryDays { get; set; }
        [DisplayName("Page Size")]
        public int PageSize { get; set; }
        [DisplayName("Pagination Enabled")] 
        public bool PaginationEnabled { get; set; }
        public int ProjectId { get; set; }
        public int TaskFilterId { get; set; }

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
            _cookies.WriteCookie(_httpContextAccessor.HttpContext.Response, "userSettings", cookieData, CookieExpiryDays);
        }

        public void WriteCookie(HttpResponse response, string setting, string settingValue, int expiryDays)
        {
            var options = new CookieOptions { Expires = new DateTimeOffset(DateTime.Now.AddDays(expiryDays)) };
            response.Cookies.Append(setting, settingValue, options);
        }

        private void InitializeSettings()
        {
            //I chose to treat each property individually in a dictionary, rather than just serialize/deserialize the SettingsModel
            //as a whole, as this would lead to an issue when adding a new setting later on. It would be impossible to 
            //workout whether the new setting is already stored in the cookie or not, as default values returned during
            //deserialization could be misleading.
            // E.g Say a new bool property is added, ShowWidget, with a config setting of 'true'. This setting would
            // not currently exist in the cookie data and would default to 'false' upon retrieval from the cook and overwrite the
            // value set by config.

            _configuration.GetSection(Position).Bind(this);

            var cookieSettings = LoadSettingsFromCookies();

            foreach (var property in typeof(SettingsModel).GetProperties())
            {
                if (cookieSettings.TryGetValue(property.Name, out var value))
                {
                    var convertedValue = Convert.ChangeType(value?.ToString(), property.PropertyType);
                    property.SetValue(this, convertedValue);
                }
            }
        }

        public T GetSetting<T>(HttpRequest request, string settingName)
        {
            var property = GetProperty<T>(settingName);
            var currentValue = property.GetValue(this);

            var qsValue = request.Query[settingName].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(qsValue))
            {
                currentValue = Convert.ChangeType(qsValue, property.PropertyType);
                property.SetValue(this, currentValue);
                SaveSettings();
            }

            return (T)currentValue;
        }

        private PropertyInfo GetProperty<T>(string settingName)
        {
            var property = typeof(SettingsModel).GetProperty(settingName);
            if (property == null)
            {
                // ReSharper disable once ConstantNullCoalescingCondition
                throw new Exception($"Property '{settingName ?? "null"}' not found");
            }

            if (property.PropertyType != typeof(T))
            {
                throw new Exception($"Property '{settingName}' is not of type {typeof(T)}");
            }

            return property;
        }

        private Dictionary<string, object> LoadSettingsFromCookies()
        {
            var settingsJson = _httpContextAccessor.HttpContext.Request.Cookies["userSettings"];
            if (!string.IsNullOrWhiteSpace(settingsJson))
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(settingsJson);
            }

            return new Dictionary<string, object>();
        }

    }
}
