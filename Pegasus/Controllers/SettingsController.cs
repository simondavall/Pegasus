using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Pegasus.Domain;
using Pegasus.Models.Settings;

namespace Pegasus.Controllers
{
    [Authorize(Roles = "PegasusUser")]
    public class SettingsController : Controller
    {
        private readonly ISettingsModel _settingsModel;
        private readonly IConfiguration _configuration;
        private readonly Cookies _cookies;

        public SettingsController(ISettingsModel settingsModel, IConfiguration configuration)
        {
            _settingsModel = settingsModel;
            _configuration = configuration;
            _cookies = new Cookies(configuration);
        }

        public IActionResult Index()
        {
            return View(_settingsModel);
        }

        [HttpPost]
        public IActionResult SaveSettings([Bind("PageSize,PaginationDisabled")] SettingsModel settings, string returnUrl)
        {
            var cookieData = SerializeProperties(settings);
            var expiryDays = GetUserSettingsExpiryDays();
            _cookies.WriteCookie(Response, "userSettings", cookieData, expiryDays);

            return Redirect(returnUrl);
        }

        private static string SerializeProperties(ISettingsModel settingsModel)
        {
            var propertyValues = new Dictionary<string, object>();
            var properties = typeof(SettingsModel).GetProperties();
            foreach (var propertyInfo in properties)
            {
                var property = propertyInfo.Name;
                var value = GetPropertyValue(settingsModel, property);
                propertyValues.Add(property, value);
            }

            return JsonSerializer.Serialize(propertyValues);
        }

        private static object GetPropertyValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName)?.GetValue(src, null);
        }

        private int GetUserSettingsExpiryDays()
        {
            try
            {
                return _configuration.GetValue<int>("Cookies:UserSettings:ExpiryDays");
            }
            catch (InvalidOperationException ex)
            {
                //TODO Implement logging to application
                //_logger.LogError(ex,
                //    "The value for 'Cookies:UserSettings:ExpiryDays' in appsettings.json must be an integer.");
                return 0;
            }
        }
    }
}
