using ReactiveUI;

namespace BookCollector.Shell
{
    public interface IShellViewModel
    {
        string DisplayName { get; set; }
        bool IsEnabled { get; set; }
        ReactiveList<IWindowCommand> LeftShellCommands { get; set; }
        ReactiveList<IWindowCommand> RightShellCommands { get; set; }
        ReactiveList<IFlyout> ShellFlyouts { get; set; }
    }
}