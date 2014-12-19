using System.IO;
using System.Reflection;
using BookCollector.Services.Browsing;
using CefSharp;

namespace BookCollector
{
    public partial class App
    {
        public App()
        {
            var settings = new CefSettings
            {
                CachePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            };

            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = SchemeHandlerFactory.Name,
                SchemeHandlerFactory = new SchemeHandlerFactory()
            });

            Cef.Initialize(settings);
        }
    }
}
