using ReactiveUI;

namespace BookCollector.Shell
{
    public class ShellScreenBase : ReactiveObject, IShellScreen
    {
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        private IFlyout _Tools;
        public IFlyout Tools
        {
            get { return _Tools; }
            protected set { this.RaiseAndSetIfChanged(ref _Tools, value); }
        }

        public virtual void Activate() { }
    }
}
