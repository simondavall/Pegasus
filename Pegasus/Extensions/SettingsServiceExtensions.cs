using System.ComponentModel;
using System.Reflection;
using Pegasus.Services;

namespace Pegasus.Extensions
{
    public static class SettingsServiceExtensions
    {
        public static string GetDisplayName(this ISettingsService service, string propertyName)
        {
            var property = service?.Settings?.GetType().GetProperty(propertyName);
            return property?.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? string.Empty;
        }
    }
}
