using BookCollector.Framework.Dialog;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Screens.Books;
using BookCollector.Screens.Collections;
using BookCollector.Screens.Main;
using BookCollector.Screens.Settings;
using BookCollector.Screens.Web;
using BookCollector.Shell;
using Ninject.Modules;

namespace BookCollector.Domain.Configuration
{
    public class ApplicationModule : NinjectModule
    {
        private ILog log = LogManager.GetCurrentClassLogger();

        public override void Load()
        {
            log.Info("Configuring Ninject");

            // Application bindings
            Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            Bind<IApplicationController>().To<ApplicationController>().InSingletonScope();
            Bind<IDialogService>().To<DialogService>().InSingletonScope();

            // View models
            Bind<IShellViewModel>().To<ShellViewModel>().InSingletonScope();
            Bind<IMainViewModel>().To<MainViewModel>().InSingletonScope();

            Bind<SettingsViewModel>().ToSelf().InSingletonScope();

            Bind<IScreen>().To<BooksViewModel>().InSingletonScope();
            Bind<SearchViewModel>().ToSelf().InSingletonScope();
            Bind<MenuViewModel>().ToSelf().InSingletonScope();

            Bind<IScreen>().To<CollectionsViewModel>().InSingletonScope();

            Bind<IScreen>().To<WebViewModel>().InSingletonScope();
        }
    }
}
