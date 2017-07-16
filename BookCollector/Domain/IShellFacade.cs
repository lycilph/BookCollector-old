using BookCollector.Framework.MVVM;
using BookCollector.Shell;

namespace BookCollector.Domain
{
    public interface IShellFacade
    {
        void Initialize();
        void AddFlyout(IFlyout flyout);
        void SetCollectionCommandVisibility(bool is_visible);
        void SetCollectionCommandText(string text);
        void SetFullscreenState(bool is_fullscreen);
        void ShowMainContent(IScreen content);
        void ShowMenuContent(IScreen content);
        void ShowHeaderContent(IScreen content);
    }
}