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

        public virtual void Activate() { }
    }
}
