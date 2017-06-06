using System.Windows;
using AutoMapper;
using BookCollector.Domain;
using BookCollector.Domain.Configuration;
using BookCollector.Framework.Logging;
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
            // Configure ninject dependency injection
            kernel = new StandardKernel(new ApplicationModule());
            // Configure Automapper
            Mapper.Initialize(cfg => cfg.AddProfile<ApplicationMappingProfile>());
            // Initialize application controller
            var application_controller = kernel.Get<IApplicationController>();
            application_controller.Initialize();
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {

        }
    }
}
