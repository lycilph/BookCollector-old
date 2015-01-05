using Caliburn.Micro.ReactiveUI;

namespace BookCollector.Screens
{
    public class ShellScreenBase : ReactiveScreen, IShellScreen
    {
        public virtual bool IsCommandsEnabled { get { return true; } }
    }
}
