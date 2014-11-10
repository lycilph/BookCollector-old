namespace BookCollector.Services.Authentication
{
    public interface IAuthenticationHandler
    {
        void Navigate(string url);
        void NavigationDone();
        void AuthorizationDone();
    }
}