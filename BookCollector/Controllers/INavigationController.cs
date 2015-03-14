namespace BookCollector.Controllers
{
    public interface INavigationController
    {
        void NavigateToMain();
        void NavigateToSearch();
        void Back();
        void NavigateToImport();
        void NavigateToSelection();
    }
}