using Caliburn.Micro.ReactiveUI;

namespace BookCollector.Screens
{
    public class BookCollectorScreenBase : ReactiveScreen, IBookCollectorScreen
    {
        public virtual bool ShowCurrentUser { get { return true; } }
    }
}
