namespace BookCollector.Controllers
{
    public interface INavigationController
    {
        void Back();
        void ResetToMain();
        void NavigateToMain();
        void NavigateToSearch();
        void NavigateToImport();
        void NavigateToSelection();
    }
}