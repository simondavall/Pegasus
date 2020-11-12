using System.ComponentModel;
using System.Reflection;
using Pegasus.Models.Settings;

namespace Pegasus.Extensions
{
    public static class SettingsModelExtensions
    {
        public static string GetDisplayName(this ISettingsModel model, string propertyName)
        {
            var property = model?.GetType().GetProperty(propertyName);
            return property?.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? string.Empty;
        }
    }
}
