namespace BookCollector.Framework.MVVM
{
    public interface IViewAware
    {
        void OnViewLoaded();
        void OnViewClosing();
    }
}
