using BookCollector.Framework.MVVM;

namespace BookCollector.Shell
{
    public interface IShellFacade
    {
        void Show();
        void ShowMainContent(IScreen content, bool is_fullscreen = false);
    }
}