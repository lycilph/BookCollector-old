using Caliburn.Micro;

namespace BookCollector.Screens
{
    public interface IShellScreen : IScreen
    {
        bool IsCommandsEnabled { get; }
    }
}
