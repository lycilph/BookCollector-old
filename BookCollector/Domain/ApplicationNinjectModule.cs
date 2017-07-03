using BookCollector.Domain.Goodreads;
using BookCollector.Framework.Dialog;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Models;
using BookCollector.Services;
using BookCollector.Services.Search;
using BookCollector.Shell;
using BookCollector.ViewModels.Screens;
using MaterialDesignThemes.Wpf;
using Ninject.Modules;

namespace BookCollector.Domain
{
    public class ApplicationNinjectModule : NinjectModule
    {
        private ILog log = LogManager.GetCurrentClassLogger();

        public override void Load()
        {
            log.Info("Configuring Ninject");

            // Framework
            Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            Bind<ISnackbarMessageQueue>().To<SnackbarMessageQueue>().InSingletonScope();
            Bind<IDialogService>().To<DialogService>().InSingletonScope();
            //Bind<IDataService>().To<DataService>().InSingletonScope();
            //Bind<IThemeService>().To<ThemeService>().InSingletonScope();
            Bind<ISearchEngine>().To<SearchEngine>().InSingletonScope();

            // Services
            Bind<INavigationService>().To<NavigationService>().InSingletonScope();
            Bind<IConfigurationService>().To<ConfigurationService>().InSingletonScope();
            Bind<IImporter>().To<GoodreadsImporter>().InSingletonScope();

            // Controllers
            Bind<IApplicationController>().To<ApplicationController>().InSingletonScope();

            // Models
            Bind<IApplicationModel>().To<ApplicationModel>().InSingletonScope();
            Bind<ICollectionModel>().To<CollectionModel>().InSingletonScope();

            // Shell
            Bind<IShellViewModel>().To<ShellViewModel>().InSingletonScope();
            Bind<IShellView>().To<ShellView>().InSingletonScope();
            Bind<IShellFacade>().To<ShellFacade>().InSingletonScope();

            // Screens
            Bind<IScreen>().To<CollectionsScreenViewModel>().InSingletonScope();
            Bind<IScreen>().To<BooksScreenViewModel>().InSingletonScope();
            //Bind<IScreen>().To<SettingsScreenViewModel>().InSingletonScope();
            Bind<IScreen>().To<ImportScreenViewModel>().InSingletonScope();
            Bind<IScreen>().To<NavigationScreenViewModel>().InSingletonScope();
            Bind<IScreen>().To<SearchScreenViewModel>().InSingletonScope();
        }
    }
}
