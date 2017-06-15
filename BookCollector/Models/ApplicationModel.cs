using BookCollector.Data;
using BookCollector.Framework.Logging;
using BookCollector.Services;

namespace BookCollector.Models
{
    public class ApplicationModel : IApplicationModel
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IDataService data_service;
        private IThemeService theme_service;

        public Settings Settings { get; set; }

        public ApplicationModel(IDataService data_service, IThemeService theme_service)
        {
            this.data_service = data_service;
            this.theme_service = theme_service;
        }

        public void Load()
        {
            log.Info("Loading");

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
            data_service.SaveSettings(Settings);
        }

        private void GetCurrentTheme()
        {
            Settings.PrimaryColor = theme_service.GetPrimaryColor();
            Settings.AccentColor = theme_service.GetAccentColor();
        }

        private void SetCurrentTheme()
        {
            theme_service.Set(Settings.PrimaryColor, Settings.AccentColor);
        }
    }
}
