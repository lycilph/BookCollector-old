using System.IO;
using System.Reflection;
using CefSharp;

namespace Mockups
{
    public partial class App
    {
        public App()
        {
            var path = Assembly.GetExecutingAssembly().Location;
            var dir = Path.GetDirectoryName(path);

            var settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(dir, "CefSharp")
            };

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
        }

        private void ApplicationExit(object sender, System.Windows.ExitEventArgs e)
        {
            Cef.Shutdown();
        }
    }
}
