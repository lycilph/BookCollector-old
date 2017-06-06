using ReactiveUI;

namespace BookCollector.Framework.MVVM
{
    public class ScreenBase : ReactiveObject, IScreen
    {
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        public virtual void Activate() { }

        public void Deactivate() { }
    }
}
