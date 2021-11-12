using Microsoft.Extensions.Configuration;

namespace PegasusService.Extensions
{
    public static class ConfigurationExtensions
    {
        public static int FromConfig(this IConfiguration config, string key, int defaultValue)
        {
            return int.TryParse(config[key], out var value) ? value : defaultValue;
        }
    }
}
