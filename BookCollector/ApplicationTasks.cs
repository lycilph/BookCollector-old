using System.ComponentModel.Composition;
using BookCollector.Controllers;
using BookCollector.Services;
using Caliburn.Micro;
using NLog;
using Panda.ApplicationCore;
using LogManager = NLog.LogManager;

namespace BookCollector
{
    public class ApplicationTasks
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [Export(ApplicationBootstrapper.STARTUP_TASK_NAME, typeof(BootstrapperTask))]
        public void ApplicationStartup()
        {
            logger.Trace("Application Startup");

            var navigation_controller = IoC.Get<INavigationController>();
            navigation_controller.NavigateToMain();

            const string csv_filename = @"C:\Private\Projects\BookCollector\Data\goodreads_export.csv";
            var books = Importer.Read(csv_filename);
        }
    }
}
