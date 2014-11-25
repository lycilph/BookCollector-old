using System.IO;
using System.Reflection;
using CefSharp;

namespace CefSharpTest
{
    public partial class App
    {
        public App()
        {
            var settings = new CefSettings {CachePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)};
            Cef.Initialize(settings);
        }
    }
}
