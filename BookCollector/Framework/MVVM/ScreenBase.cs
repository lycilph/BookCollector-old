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

        private bool _IsActive = false;
        public bool IsActive
        {
            get { return _IsActive; }
            protected set { this.RaiseAndSetIfChanged(ref _IsActive, value); }
        }

        public virtual void Activate()
        {
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }
    }
}
