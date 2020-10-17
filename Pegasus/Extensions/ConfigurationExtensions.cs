using Microsoft.Extensions.Configuration;

namespace Pegasus.Extensions
{
    public static class ConfigurationExtensions
    {
        public static int FromConfig(this IConfiguration self, string key, int defaultValue)
        {
            int value = int.TryParse(self[key], out value)
                ? value
                : defaultValue;
            return value;
        }

        public static string FromConfig(this IConfiguration self, string key, string defaultValue)
        {
            return self[key] ?? defaultValue;
        }
    }
}
