using BookCollector.Data;

namespace BookCollector.Services
{
    public class SettingsService : ISettingsService
    {
        private ISettingsRepository settings_repository;
        private IThemeService theme_service;

        public Settings Settings { get; private set; }

        public SettingsService(ISettingsRepository settings_repository, IThemeService theme_service)
        {
            this.settings_repository = settings_repository;
            this.theme_service = theme_service;
        }

        public void Initialize()
        {
            Settings = settings_repository.LoadOrCreate();
            theme_service.Set(Settings.PrimaryColor, Settings.AccentColor);
        }

        public void Exit()
        {
            settings_repository.Save(Settings);
        }
    }
}
