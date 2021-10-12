using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Pegasus.Domain;
using Pegasus.Library.JwtAuthentication.Constants;
using Pegasus.Library.Services.Http;
using Pegasus.Services.Models;

namespace Pegasus.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IHttpContextWrapper _httpContext;
        private readonly IConfiguration _configuration;
        private const string ConfigSection = "PegasusSettings";
        private readonly ICookies _cookies;

        public SettingsService()
        {
            Settings = new SettingsModel();
        }

        public SettingsService(IHttpContextWrapper httpContextWrapper, IConfiguration configuration)
        {
            _httpContext = httpContextWrapper;
            _configuration = configuration;
            InitializeSettings();
            _cookies = new Cookies(_httpContext, this);
        }

        public SettingsModel Settings { get; set; }

        public T GetSetting<T>(string settingName)
        {
            var request = _httpContext.Request;
            var property = GetProperty<T>(settingName);
            var currentValue = property.GetValue(Settings);

            var qsValue = request.Query[settingName].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(qsValue))
            {
                currentValue = Convert.ChangeType(qsValue, property.PropertyType);
                property.SetValue(Settings, currentValue);
                SaveSettings();
            }

            return (T)currentValue;
        }

        public void SaveSettings()
        {
            var propertyValues = new Dictionary<string, object>();
            var properties = typeof(SettingsModel).GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(Settings, null);
                propertyValues.Add(property.Name, value);
            }

            var cookieData = JsonSerializer.Serialize(propertyValues);
            _cookies.WriteCookie(CookieConstants.UserSettings, cookieData, Settings.CookieExpiryDays);
        }

        private void InitializeSettings()
        {
            //I chose to treat each property individually in a dictionary, rather than just serialize/deserialize the SettingsModel
            //as a whole, as this would lead to an issue when adding a new setting later on. It would be impossible to 
            //workout whether the new setting is already stored in the cookie or not, as default values returned during
            //deserialization could be misleading.
            // E.g Say a new bool property is added, ShowWidget, with a config setting of 'true'. This setting would
            // not currently exist in the cookie data and would default to 'false' upon retrieval from the cookie and overwrite the
            // value set by config.

            Settings ??= new SettingsModel();
            _configuration.GetSection(ConfigSection).Bind(Settings);

            var cookieSettings = LoadSettingsFromCookies();

            foreach (var property in typeof(SettingsModel).GetProperties())
            {
                if (cookieSettings.TryGetValue(property.Name, out var value))
                {
                    var convertedValue = Convert.ChangeType(value?.ToString(), property.PropertyType);
                    property.SetValue(Settings, convertedValue);
                }
            }
        }

        private PropertyInfo GetProperty<T>(string settingName)
        {
            var property = typeof(SettingsModel).GetProperty(settingName);
            if (property == null)
            {
                // ReSharper disable once ConstantNullCoalescingCondition
                throw new PropertyNotFoundException($"Property '{settingName ?? "null"}' not found");
            }

            if (property.PropertyType != typeof(T))
            {
                throw new PropertyTypeInvalidException($"Property '{settingName}' is not of type {typeof(T)}");
            }

            return property;
        }

        private Dictionary<string, object> LoadSettingsFromCookies()
        {
            var settingsJson = _httpContext.Request.Cookies[CookieConstants.UserSettings];
            if (!string.IsNullOrWhiteSpace(settingsJson))
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(settingsJson);
            }

            return new Dictionary<string, object>();
        }
    }

    public class PropertyNotFoundException : Exception
    {
        public PropertyNotFoundException(string message):base(message)
        { }
    }

    public class PropertyTypeInvalidException : Exception
    {
        public PropertyTypeInvalidException(string message):base(message)
        { }
    }
}
