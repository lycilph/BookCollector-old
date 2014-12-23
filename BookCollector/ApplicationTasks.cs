using System.ComponentModel.Composition;
using Caliburn.Micro;
using Framework.Core;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector
{
    public class ApplicationTasks
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [Export(ApplicationBootstrapper.STARTUP_TASK_NAME, typeof (BootstrapperTask))]
        public void ActivateApplicationController()
        {
            logger.Trace("Activating application controller");
            var application_controller = IoC.Get<ApplicationController>();
            application_controller.Activate();
        }

        [Export(ApplicationBootstrapper.SHUTDOWN_TASK_NAME, typeof(BootstrapperTask))]
        public void DeactivateApplicationController()
        {
            logger.Trace("Deactivating application controller");
            var application_controller = IoC.Get<ApplicationController>();
            application_controller.Deactivate();
        }
    }
}
