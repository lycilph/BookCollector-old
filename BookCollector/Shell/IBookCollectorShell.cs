using Caliburn.Micro;
using Panda.ApplicationCore.Shell;

namespace BookCollector.Shell
{
    public interface IBookCollectorShell : IShell, IHaveDisplayName
    {
        void Back();
        void Show(IScreen screen);
    }
}
