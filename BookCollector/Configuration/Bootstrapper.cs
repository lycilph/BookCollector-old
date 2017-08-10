using System;
using System.Windows;
using BookCollector.Screens.Shell;
using BookCollector.Services;
using Core.Application;
using Ninject;
using NLog;

namespace BookCollector.Configuration
{
    public class Bootstrapper : BootstrapperBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public IKernel Kernel { get; private set; }

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            logger.Trace("Configure");

            ConfigureNinject();
            ConfigureNavigation();
            ConfigureObjectMapping();
        }

        private void ConfigureNinject()
        {
            logger.Trace("Configuring Ninject Kernel");

            Kernel = new StandardKernel(new ApplicationNinjectModule());
        }

        private void ConfigureNavigation()
        {
            logger.Trace("Configuring navigation");

            var navigation_service = Kernel.Get<INavigationService>();
            NavigationConfiguration.Setup(navigation_service);
        }
        
        private void ConfigureObjectMapping()
        {
            logger.Trace("Configuring object mapping");

            ObjectMappingConfiguration.Setup();
        }

        protected override object GetInstance(Type service)
        {
            logger.Trace($"Getting instance for {service.Name} from Ninject Kernel");

            return Kernel.Get(service);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            logger.Trace("Startup");

            var controller = Kernel.Get<IApplicationController>();
            controller.Initialize();

            DisplayRootViewFor<IShellViewModel>();
        }

        protected override void OnExit(object sender, ExitEventArgs e)
        {
            logger.Trace("Exit");

            var controller = Kernel.Get<IApplicationController>();
            controller.Exit();
        }
    }
}
