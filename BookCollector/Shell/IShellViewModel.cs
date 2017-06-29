using ReactiveUI;
using IScreen = BookCollector.Framework.MVVM.IScreen;

namespace BookCollector.Shell
{
    public interface IShellViewModel : IScreen
    {
        bool IsFullscreen { get; set; }

        ReactiveList<IWindowCommand> LeftShellCommands { get; }
        ReactiveList<IWindowCommand> RightShellCommands { get; }
        ReactiveList<IFlyout> ShellFlyouts { get; }

        void ShowMainContent(IScreen content);
        void ShowMenuContent(IScreen content);
        void ShowHeaderContent(IScreen content);
    }
}