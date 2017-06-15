using MaterialDesignThemes.Wpf;

namespace BookCollector.Services
{
    public class ThemeService : IThemeService
    {
        private PaletteHelper palette_helper = new PaletteHelper();

        public string GetPrimaryColor()
        {
            return palette_helper.QueryPalette().PrimarySwatch.Name;
        }

        public string GetAccentColor()
        {
            return palette_helper.QueryPalette().AccentSwatch.Name;
        }

        public void Set(string primary_color, string accent_color)
        {
            palette_helper.ReplacePrimaryColor(primary_color);
            palette_helper.ReplaceAccentColor(accent_color);
        }
    }
}
