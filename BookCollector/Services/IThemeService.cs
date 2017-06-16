using System.Collections.Generic;
using MaterialDesignColors;

namespace BookCollector.Services
{
    public interface IThemeService
    {
        string GetCurrentPrimaryColor();
        string GetCurrentAccentColor();

        List<Swatch> GetAllPrimaryColors();
        List<Swatch> GetAllAccentColors();

        void Set(string primary_color, string accent_color);
        void SetPrimary(string color);
        void SetAccent(string color);
    }
}