namespace BookCollector.Services
{
    public interface INavigationService
    {
        void Initialize();
        void NavigateTo(string screen_name);
    }
}