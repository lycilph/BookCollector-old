namespace BookCollector.Services
{
    public interface IThemeService
    {
        string GetPrimaryColor();
        string GetAccentColor();
        void Set(string primary_color, string accent_color);
    }
}