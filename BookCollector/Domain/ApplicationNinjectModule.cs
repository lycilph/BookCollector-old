using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Shell;
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

            Bind<IApplicationController>().To<ApplicationController>();

            Bind<IShellViewModel>().To<ShellViewModel>();
            Bind<IShellView>().To<ShellView>();
        }
    }
}
