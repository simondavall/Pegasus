using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Pegasus.Domain
{
    public class SettingsAccessor
    {
        private readonly IConfiguration _configuration;

        public SettingsAccessor(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// This attempts to return a setting value from the following places, in this order
        ///     - QueryString
        ///     - Cookie
        ///     - Appsettings.json
        ///     - Returns default value supplied.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="settingName"></param>
        /// <param name="defaultReturnVal"></param>
        /// <returns></returns>
        public T GetSetting<T>(HttpRequest request, string settingName, T defaultReturnVal = default)
        {
            if (request != null)
            {
                if (TryGetFromQueryString(request, settingName, out T id))
                    return id;

                if (TryGetFromCookie(request, settingName, out id))
                    return id;
            }

            return GetSetting(settingName, defaultReturnVal);
        }

        /// <summary>
        /// This attempts to return a setting value from the following places, in this order
        ///     - Appsettings.json
        ///     - Returns default value supplied. 
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="defaultReturnVal"></param>
        /// <returns></returns>
        public T GetSetting<T>(string settingName, T defaultReturnVal = default)
        {
            return GetFromConfiguration(settingName, defaultReturnVal);
        }

        private bool TryGetFromQueryString<T>(HttpRequest request, string settingName, out T value)
        {
            var qs = request.Query[settingName].FirstOrDefault();
            return ConvertTo(qs, out value);
        }

        private bool TryGetFromCookie<T>(HttpRequest request, string settingName, out T value)
        {
            var cookieValue = request.Cookies[settingName];
            return ConvertTo(cookieValue, out value);
        }

        private bool ConvertTo<T>(string input, out T value)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(input))
                {
                    value = (T) Convert.ChangeType(input, typeof(T));
                    return true;
                }

                value = default;
                return false;
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
        }

        private T GetFromConfiguration<T>(string settingName, T defaultValue)
        {
            try
            {
                return _configuration.GetValue($"ToBeRemoved:{settingName}", defaultValue);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}
