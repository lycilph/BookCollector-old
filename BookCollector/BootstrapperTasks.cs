using System.ComponentModel.Composition;
using BookCollector.Data;
using BookCollector.Goodreads;
using Caliburn.Micro;
using Framework.Core;

namespace BookCollector
{
    public class BootstrapperTasks
    {
        [Export(ApplicationBootstrapper.STARTUP_TASK_NAME, typeof(BootstrapperTask))]
        public void Load()
        {
            var settings = IoC.Get<ApplicationSettings>();
            settings.Load();

            var repository = IoC.Get<InfoRepository>();
            repository.Load();

            var api = IoC.Get<GoodreadsApi>();
            api.Initialize();
        }

        [Export(ApplicationBootstrapper.SHUTDOWN_TASK_NAME, typeof(BootstrapperTask))]
        public void Save()
        {
            var settings = IoC.Get<ApplicationSettings>();
            settings.Save();

            var repository = IoC.Get<InfoRepository>();
            repository.Save();

            var api = IoC.Get<GoodreadsApi>();
            api.Shutdown();
        }
    }
}
