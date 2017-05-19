using System.Windows;
using Caliburn.Micro;

namespace BookCollector.Shell
{
    public class Bootstrapper : BootstrapperBase
    {
        private ILog log;

        static Bootstrapper()
        {
            LogManager.GetLog = type => new DebugLog(type);
        }

        public Bootstrapper()
        {
            log = LogManager.GetLog(typeof(Bootstrapper));

            log.Info("Initializeing bootstrapper");
            Initialize();
        }

        protected override void Configure()
        {
            log.Info("Configuring bootstrapper");
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
