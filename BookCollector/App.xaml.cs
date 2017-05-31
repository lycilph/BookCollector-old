using System.Windows;
using AutoMapper;
using BookCollector.Application.Configuration;
using BookCollector.Controllers;
using BookCollector.Framework.Logging;
using BookCollector.Shell;
using Ninject;

namespace BookCollector
{
    public partial class App : System.Windows.Application
    {
        private ILog log;
        private IKernel kernel;

        public App()
        {
            LogManager.GetLog = type => new DebugLog(type);
            log = LogManager.GetCurrentClassLogger();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            log.Info("Application starting");

            kernel = new StandardKernel(new ApplicationModule());
            Mapper.Initialize(cfg => cfg.AddProfile<MappingProfile>());

            InitializeApplicationController();
            ShowShell();
        }

        private void InitializeApplicationController()
        {
            log.Info("Initializing application controller");

            var application_controller = kernel.Get<IApplicationController>();
            application_controller.Initialize();
        }

        private void ShowShell()
        {
            log.Info("Showing the shell");

            var shell = kernel.Get<ShellViewModel>();
            var shell_view = new ShellView() { DataContext = shell };

            shell_view.Show();
        }
    }
}
