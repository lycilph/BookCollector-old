using BookCollector.Framework.MVVM;
using ReactiveUI;
using IScreen = BookCollector.Framework.MVVM.IScreen;

namespace BookCollector.Shell
{
    public interface IShellViewModel : IScreen
    {
        ReactiveList<IWindowCommand> LeftShellCommands { get; }
        ReactiveList<IWindowCommand> RightShellCommands { get; }
        ReactiveList<IFlyout> ShellFlyouts { get; }

        void Show(IScreen content);
    }
}