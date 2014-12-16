using CefSharp;

namespace BookCollector.Services.Browsing
{
    public class SchemeHandlerFactory : ISchemeHandlerFactory
    {
        public const string Name = "custom";

        public ISchemeHandler Create()
        {
            return new SchemeHandler();
        }
    }
}
