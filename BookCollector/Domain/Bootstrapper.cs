using BookCollector.Framework.Logging;
using Ninject;

namespace BookCollector.Domain
{
    public class Bootstrapper
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IKernel kernel;

        public void Startup()
        {
            log.Info("Startup");
            SetupObjectMapping();
            SetupDependencyInjection();
            InitializeApplicationController();
        }

        public void Exit()
        {
            log.Info("Exit");
        }

        private void SetupObjectMapping()
        {
            log.Info("Setting up object mappings");
            ApplicationObjectMapping.Setup();
        }

        private void SetupDependencyInjection()
        {
            log.Info("Setting up dependency injection");
            kernel = new StandardKernel(new ApplicationNinjectModule());
        }

        private void InitializeApplicationController()
        {
            log.Info("Initialize application controller");
            var application_controller = kernel.Get<IApplicationController>();
            application_controller.Initialize();
        }
    }
}
