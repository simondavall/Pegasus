using Pegasus.Services.Models;

namespace Pegasus.Services
{
    public interface ISettingsService
    {
        public SettingsModel Settings { get; set; }

        T GetSetting<T>(string settingName);
        void SaveSettings();
    }
}