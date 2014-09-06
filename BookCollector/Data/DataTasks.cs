using System.ComponentModel.Composition;
using Caliburn.Micro;
using Framework.Core;

namespace BookCollector.Data
{
    public class DataTasks
    {
        [Export(ApplicationBootstrapper.STARTUP_TASK_NAME, typeof(BootstrapperTask))]
        public void Load()
        {
            var settings = IoC.Get<ApplicationSettings>();
            settings.Load();

            var repository = IoC.Get<InfoRepository>();
            repository.Load();
        }

        [Export(ApplicationBootstrapper.SHUTDOWN_TASK_NAME, typeof(BootstrapperTask))]
        public void Save()
        {
            var settings = IoC.Get<ApplicationSettings>();
            settings.Save();

            var repository = IoC.Get<InfoRepository>();
            repository.Save();
        }
    }
}
