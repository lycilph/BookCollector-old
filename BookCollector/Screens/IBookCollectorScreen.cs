using Caliburn.Micro;

namespace BookCollector.Screens
{
    public interface IBookCollectorScreen : IScreen
    {
        bool ShowCurrentUser { get; }
    }
}
