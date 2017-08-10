using ReactiveUI;

namespace Core.Shell
{
    public interface IWindowCommand
    {
        object DisplayName { get; set; }
        bool IsEnabled { get; set; }
        bool IsVisible { get; set; }
        ReactiveCommand ExecuteCommand { get; }
    }
}
