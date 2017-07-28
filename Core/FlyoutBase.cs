using MahApps.Metro.Controls;
using ReactiveUI;

namespace Core
{
    public class FlyoutBase : ScreenBase, IFlyout
    {
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
