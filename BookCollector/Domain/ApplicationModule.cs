using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Shell;
using Ninject.Modules;

namespace BookCollector.Domain
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
            Bind<IShellViewModel>().To<ShellViewModel>().InSingletonScope();
        }
    }
}
