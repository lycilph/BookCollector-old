using BookCollector.Framework.MVVM;

namespace BookCollector.Services
{
    public interface IConfigurationService
    {
        ScreenConfiguration Get(string screen_name);
    }
}