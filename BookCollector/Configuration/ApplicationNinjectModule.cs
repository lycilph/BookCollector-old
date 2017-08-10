using BookCollector.Screens.Books;
using BookCollector.Screens.Collections;
using BookCollector.Screens.Import;
using BookCollector.Screens.Settings;
using BookCollector.Screens.Shell;
using BookCollector.Screens.Web;
using BookCollector.Services;
using Core.Dialogs;
using Core.Shell;
using MaterialDesignThemes.Wpf;
using Ninject.Modules;
using NLog;

namespace BookCollector.Configuration
{
    public class ApplicationNinjectModule : NinjectModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override void Load()
        {
            logger.Trace("Loading dependencies for application");

            Bind<IApplicationController>().To<ApplicationController>().InSingletonScope();
            Bind<ISnackbarMessageQueue>().To<SnackbarMessageQueue>().InSingletonScope();

            Bind<ISettingsRepository>().To<SettingsRepository>().InSingletonScope();
            Bind<ICollectionsRepository>().To<CollectionsRepository>().InSingletonScope();

            Bind<INavigationService>().To<NavigationService>().InSingletonScope();
            Bind<ICollectionsService>().To<CollectionsService>().InSingletonScope();
            Bind<IImportService>().To<ImportService>().InSingletonScope();
            Bind<ISettingsService>().To<SettingsService>().InSingletonScope();
            Bind<IThemeService>().To<ThemeService>().InSingletonScope();
            Bind<IDialogService>().To<DialogService>().InSingletonScope();

            Bind<IShellViewModel>().To<ShellViewModel>().InSingletonScope();

            Bind<IScreen>().To<CollectionsScreenViewModel>();
            Bind<IScreen>().To<BooksScreenViewModel>();
            Bind<IScreen>().To<ImportScreenViewModel>();
            Bind<IScreen>().To<SettingsScreenViewModel>();
            Bind<IScreen>().To<WebScreenViewModel>();
            Bind<IScreen>().To<SearchScreenViewModel>();
            Bind<IScreen>().To<NavigationScreenViewModel>();
        }
    }
}
