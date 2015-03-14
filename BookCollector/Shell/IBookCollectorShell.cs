using BookCollector.Screens;
using Caliburn.Micro;
using Panda.ApplicationCore.Shell;

namespace BookCollector.Shell
{
    public interface IBookCollectorShell : IShell, IHaveDisplayName, IHaveActiveItem
    {
        void Back();
        void Show(IBookCollectorScreen screen);
    }
}
