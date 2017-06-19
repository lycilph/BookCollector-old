using ReactiveUI;
using IScreen = BookCollector.Framework.MVVM.IScreen;

namespace BookCollector.Shell
{
    public interface IShellViewModel : IScreen
    {
        ReactiveList<IWindowCommand> LeftShellCommands { get; }
        ReactiveList<IWindowCommand> RightShellCommands { get; }
        ReactiveList<IFlyout> ShellFlyouts { get; }

        void ShowMainContent(IScreen content, bool is_fullscreen);
        void ShowMenuContent(IScreen content);
        void ShowHeaderContent(IScreen content);
    }
}