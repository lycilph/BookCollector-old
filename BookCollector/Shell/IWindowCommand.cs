using ReactiveUI;

namespace BookCollector.Shell
{
    public interface IWindowCommand
    {
        bool IsEnabled { get; set; }
        bool IsVisible { get; set; }
        ReactiveCommand ExecuteCommand { get; }
    }
}
