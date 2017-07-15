using System.Collections.Generic;
using BookCollector.Framework.MVVM;

namespace BookCollector.Services
{
    public interface IConfigurationService
    {
        ScreenConfiguration Get(string screen_name);
        List<ScreenConfiguration> GetFlyouts();
    }
}