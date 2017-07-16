using System.Collections.Generic;
using System.Linq;
using BookCollector.Domain;
using BookCollector.Framework.MVVM;

namespace BookCollector.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private Dictionary<string, ScreenConfiguration> screen_configurations;

        public ConfigurationService()
        {
            screen_configurations = new Dictionary<string, ScreenConfiguration>()
            {
                { Constants.ImportScreenDisplayName, new ScreenConfiguration(Constants.ImportScreenDisplayName, false, true) },
                { Constants.BooksScreenDisplayName, new ScreenConfiguration(Constants.BooksScreenDisplayName, Constants.SearchScreenDisplayName, Constants.NavigationScreenDisplayName, false, true) },
                { Constants.CollectionsScreenDisplayName, new ScreenConfiguration(Constants.CollectionsScreenDisplayName, false, false ) },
                { Constants.SettingsScreenDisplayName, new ScreenConfiguration(Constants.SettingsScreenDisplayName, true) },
                { Constants.WebScreenDisplayName, new ScreenConfiguration(Constants.WebScreenDisplayName, true, false) }
            };
        }

        public ScreenConfiguration Get(string screen_name)
        {
            if (!screen_configurations.TryGetValue(screen_name, out ScreenConfiguration configuration))
                throw new System.ArgumentException("screen_name");
            return configuration;
        }

        public List<ScreenConfiguration> GetFlyouts()
        {
            return screen_configurations.Values.Where(c => c.IsFlyout).ToList();
        }
    }
}
