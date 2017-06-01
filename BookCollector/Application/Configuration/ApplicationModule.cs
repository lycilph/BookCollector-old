using BookCollector.Controllers;
using BookCollector.Data;
using BookCollector.Framework.Logging;
using BookCollector.Framework.MessageBus;
using BookCollector.Models;
using BookCollector.Screens.Main;
using BookCollector.Screens.Start;
using BookCollector.Shell;
using Ninject.Modules;

namespace BookCollector.Application.Configuration
{
    public class ApplicationModule : NinjectModule
    {
        private ILog log = LogManager.GetCurrentClassLogger();

        public override void Load()
        {
            log.Info("Configuring Ninject");

            // Application bindings
            Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            Bind<IDialogService>().To<DialogService>().InSingletonScope();
            Bind<IApplicationController>().To<ApplicationController>().InSingletonScope();
            Bind<IDataController>().To<DataController>().InSingletonScope();

            // Data bindings
            Bind<Settings>().ToSelf().InSingletonScope();

            // Model bindings
            Bind<IBookCollectorModel>().To<BookCollectorModel>().InSingletonScope();

            // ViewModel bindings
            Bind<IShellScreen>().To<StartViewModel>().InSingletonScope();
            Bind<IShellScreen>().To<MainViewModel>().InSingletonScope();

            Bind<MainToolsViewModel>().ToSelf().InSingletonScope();
        }
    }
}
