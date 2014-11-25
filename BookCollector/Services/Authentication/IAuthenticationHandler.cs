namespace BookCollector.Services.Authentication
{
    public interface IAuthenticationHandler
    {
        void Navigate(string url);
        dynamic GetDocument();
        void NavigationDone();
        void AuthorizationDone();
    }
}