using System;
using System.Reactive.Linq;
using BookCollector.Framework.MVVM;
using MahApps.Metro.Controls;
using ReactiveUI;
using IScreen = BookCollector.Framework.MVVM.IScreen;

namespace BookCollector.Shell
{
    public class FlyoutBase : ScreenBase, IFlyout, IScreen
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

        public FlyoutBase(string name, Position position)
        {
            DisplayName = name;
            _Position = position;

            this.WhenAnyValue(x => x.IsOpen)
                .Subscribe(is_open =>
                {
                    if (is_open)
                        Activate();
                    else
                        Deactivate();
                });
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
