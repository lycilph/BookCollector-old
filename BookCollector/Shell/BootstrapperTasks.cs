using System.ComponentModel.Composition;
using Framework.Core;

namespace BookCollector.Shell
{
    public class BootstrapperTasks
    {
        [Export(ApplicationBootstrapper.STARTUP_TASK_NAME, typeof(BootstrapperTask))]
        public void LoadSettings()
        {
            //var profiles = IoC.Get<ProfileRepository>();
            //profiles.Load();
        }
    }
}
