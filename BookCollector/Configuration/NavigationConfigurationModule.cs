using BookCollector.Screens.Books;
using BookCollector.Screens.Collections;
using BookCollector.Screens.Import;
using BookCollector.Screens.Settings;
using BookCollector.Screens.Shell;
using BookCollector.Screens.Web;
using BookCollector.Services;

namespace BookCollector.Configuration
{
    public class NavigationConfigurationModule
    {
        public static void Setup(INavigationService navigation_service)
        {
            navigation_service.Register(typeof(IBooksScreen));
            navigation_service.Register(typeof(ICollectionsScreen), false /*show_collection_command*/);
            navigation_service.Register(typeof(IImportScreen));
            navigation_service.Register(typeof(ISettingsScreen));
            navigation_service.Register(typeof(IWebScreen), ShellScreenPosition.MainContent, true, true);
            navigation_service.Register(typeof(ISearchScreen), ShellScreenPosition.HeaderContent);
            navigation_service.Register(typeof(INavigationScreen), ShellScreenPosition.MenuContent);
        }
    }
}
