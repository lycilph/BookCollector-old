using System.IO;
using System.Reflection;
using System.Windows;
using BookCollector.Domain;
using BookCollector.Domain.Configuration;
using BookCollector.Framework.Logging;
using CefSharp;
using Ninject;

namespace BookCollector
{
    // This bootstraps the application
    public partial class App
    {
        private ILog log;
        private IKernel kernel;

        public App()
        {
            LogManager.GetLog = type => new DebugLog(type);
            log = LogManager.GetCurrentClassLogger();
        }

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            // Setup CEF
            SetupCEF();
            // Setup object mapping
            ApplicationObjectMapping.Setup();
            // Configure ninject dependency injection
            kernel = new StandardKernel(new ApplicationModule());
            // Initialize application controller
            var application_controller = kernel.Get<IApplicationController>();
            application_controller.Initialize();
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            Cef.Shutdown();
        }

        private void SetupCEF()
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
    }
}
