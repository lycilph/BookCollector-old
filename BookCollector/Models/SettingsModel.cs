using BookCollector.Data;
using BookCollector.Framework.Logging;
using BookCollector.Services;
using ReactiveUI;

namespace BookCollector.Models
{
    public class SettingsModel : ReactiveObject, ISettingsModel
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IDataService data_service;
        private IThemeService theme_service;

        public Settings Settings { get; private set; }

        public SettingsModel(IDataService data_service, IThemeService theme_service)
        {
            this.data_service = data_service;
            this.theme_service = theme_service;
        }

        public void Load()
        {
            log.Info("Loading");

            // Handle settings
            if (data_service.SettingsExists())
            {
                Settings = data_service.LoadSettings();
                SetCurrentTheme();
            }
            else
            {
                Settings = new Settings();
                GetCurrentTheme();
            }
        }

        public void Save()
        {
            log.Info("Saving");

            data_service.SaveSettings(Settings);
        }

        private void GetCurrentTheme()
        {
            Settings.PrimaryColor = theme_service.GetCurrentPrimaryColor();
            Settings.AccentColor = theme_service.GetCurrentAccentColor();
        }

        private void SetCurrentTheme()
        {
            theme_service.Set(Settings.PrimaryColor, Settings.AccentColor);
        }
    }
}
