using Caliburn.Micro;

namespace BookCollector.Shell
{
    public interface IWindowCommand : IHaveDisplayName
    {
        bool IsEnabled { get; set; }
        bool IsVisible { get; set; }
        void Execute();
    }
}
