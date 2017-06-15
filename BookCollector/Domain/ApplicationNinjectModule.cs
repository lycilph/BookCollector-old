using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Models;
using BookCollector.Services;
using BookCollector.Shell;
using BookCollector.ViewModels.Screens;
using Ninject.Modules;

namespace BookCollector.Domain
{
    public class ApplicationNinjectModule : NinjectModule
    {
        private ILog log = LogManager.GetCurrentClassLogger();

        public override void Load()
        {
            log.Info("Configuring Ninject");

            Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            Bind<IDataService>().To<DataService>().InSingletonScope();
            Bind<IThemeService>().To<ThemeService>().InSingletonScope();

            Bind<IApplicationController>().To<ApplicationController>().InSingletonScope();
            Bind<IApplicationModel>().To<ApplicationModel>().InSingletonScope();

            Bind<IShellViewModel>().To<ShellViewModel>().InSingletonScope();
            Bind<IShellView>().To<ShellView>().InSingletonScope();
            Bind<IShellFacade>().To<ShellFacade>().InSingletonScope();

            Bind<IScreen>().To<CollectionsScreenViewModel>().InSingletonScope();
            Bind<IScreen>().To<BooksScreenViewModel>().InSingletonScope();
        }
    }
}
