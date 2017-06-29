using BookCollector.Framework.MVVM;

namespace BookCollector.Domain
{
    public interface IShellFacade
    {
        void Initialize();
        void SetCollectionCommandVisibility(bool is_visible);
        void SetFullscreenState(bool is_fullscreen);
        void ShowShell();
        void ShowMainContent(IScreen content);
        void ShowMenuContent(IScreen content);
        void ShowHeaderContent(IScreen content);
    }
}