using System.Collections.Generic;
using System.Linq;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace BookCollector.Services
{
    public class ThemeService : IThemeService
    {
        private PaletteHelper palette_helper = new PaletteHelper();
        private SwatchesProvider swatches_provider = new SwatchesProvider();

        public string GetCurrentPrimaryColor()
        {
            return palette_helper.QueryPalette().PrimarySwatch.Name;
        }

        public string GetCurrentAccentColor()
        {
            return palette_helper.QueryPalette().AccentSwatch.Name;
        }

        public List<Swatch> GetAllPrimaryColors()
        {
            return swatches_provider.Swatches.ToList();
        }

        public List<Swatch> GetAllAccentColors()
        {
            return swatches_provider.Swatches.Where(s => s.IsAccented).ToList();
        }

        public void Set(string primary_color, string accent_color)
        {
            palette_helper.ReplacePrimaryColor(primary_color);
            palette_helper.ReplaceAccentColor(accent_color);
        }
        public void SetPrimary(string color)
        {
            palette_helper.ReplacePrimaryColor(color);
        }

        public void SetAccent(string color)
        {
            palette_helper.ReplaceAccentColor(color);
        }
    }
}
