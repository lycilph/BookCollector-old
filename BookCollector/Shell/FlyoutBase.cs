using MahApps.Metro.Controls;
using ReactiveUI;

namespace BookCollector.Shell
{
    public class FlyoutBase : ReactiveObject, IFlyout
    {
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        private bool _IsOpen;
        public bool IsOpen
        {
            get { return _IsOpen; }
            set { this.RaiseAndSetIfChanged(ref _IsOpen, value); }
        }

        private bool _IsPinned;
        public bool IsPinned
        {
            get { return _IsPinned; }
            set { this.RaiseAndSetIfChanged(ref _IsPinned, value); }
        }

        private Position _Position;
        public Position Position
        {
            get { return _Position; }
            set { this.RaiseAndSetIfChanged(ref _Position, value); }
        }

        public FlyoutBase(string name, Position position)
        {
            _DisplayName = name;
            _Position = position;
        }

        public void Toggle()
        {
            IsOpen = !IsOpen;
        }

        public void Show()
        {
            IsOpen = true;
        }

        public void Hide()
        {
            IsOpen = false;
        }
    }
}
