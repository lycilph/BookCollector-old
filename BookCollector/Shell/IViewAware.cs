namespace BookCollector.Shell
{
    public interface IViewAware
    {
        void OnViewLoaded();
        void OnViewClosing();
    }
}
