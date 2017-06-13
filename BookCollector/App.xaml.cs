using System.Windows;
using BookCollector.Domain;
using BookCollector.Framework.Logging;

namespace BookCollector
{
    public partial class App
    {
        private ILog log;
        private Bootstrapper bootstrapper;

        public App()
        {
            LogManager.GetLog = type => new DebugLog(type);
            log = LogManager.GetCurrentClassLogger();

            bootstrapper = new Bootstrapper();
        }

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            log.Info("Startup");
            bootstrapper.Startup();
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            log.Info("Exit");
            bootstrapper.Exit();
        }
    }
}
