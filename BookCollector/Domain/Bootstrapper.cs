using System.IO;
using System.Reflection;
using BookCollector.Framework.Logging;
using CefSharp;
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
            SetupCEF();
            SetupObjectMapping();
            SetupDependencyInjection();
            InitializeApplicationController();
        }

        public void Exit()
        {
            log.Info("Exit");
            ShutdownCEF();
        }

        public void SetupCEF()
        {
            var path = Assembly.GetExecutingAssembly().Location;
            var dir = Path.GetDirectoryName(path);

            var settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(dir, "CefSharp"),
            };
            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
        }

        public void ShutdownCEF()
        {
            Cef.Shutdown();
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
