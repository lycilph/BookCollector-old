using BookCollector.Controllers;
using BookCollector.Framework.Logging;
using BookCollector.Framework.MessageBus;
using BookCollector.Models;
using BookCollector.Screens.Main;
using BookCollector.Screens.Start;
using BookCollector.Shell;
using Ninject.Modules;

namespace BookCollector.Initialization
{
    public class ApplicationModule : NinjectModule
    {
        private ILog log = LogManager.GetCurrentClassLogger();

        public override void Load()
        {
            log.Info("Configuring Ninject");

            Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            Bind<IBookCollectorModel>().To<BookCollectorModel>().InSingletonScope();
            Bind<ApplicationController>().ToSelf().InSingletonScope();

            Bind<IShellScreen>().To<StartViewModel>().InSingletonScope();
            Bind<IShellScreen>().To<MainViewModel>().InSingletonScope();
        }
    }
}
