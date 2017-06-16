using BookCollector.Framework.MVVM;

namespace BookCollector.Shell
{
    public interface IShellFacade
    {
        void AddCommand(IWindowCommand command, ShellFacade.CommandPosition position);
        void AddFlyout(IFlyout flyout);

        void Show();
        void ShowMainContent(IScreen content, bool is_fullscreen = false);
    }
}