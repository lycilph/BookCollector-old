using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace Core.Shell
{
    public interface IShell
    {
        ReactiveList<IWindowCommand> LeftShellCommands { get; set; }
        ReactiveList<IWindowCommand> RightShellCommands { get; set; }
        ReactiveList<IFlyout> ShellFlyouts { get; set; }
        ISnackbarMessageQueue MessageQueue { get; set; }
    }
}