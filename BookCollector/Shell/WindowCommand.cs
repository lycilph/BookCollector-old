using System;
using ReactiveUI;

namespace BookCollector.Shell
{
    public class WindowCommand : ReactiveObject, IWindowCommand
    {
        private readonly Action action;

        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        private bool _IsEnabled = true;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { this.RaiseAndSetIfChanged(ref _IsEnabled, value); }
        }

        private bool _IsVisible = true;
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { this.RaiseAndSetIfChanged(ref _IsVisible, value); }
        }

        public WindowCommand(string name, Action action)
        {
            this.action = action;
            DisplayName = name;
        }

        public virtual void Execute()
        {
            action();
        }
    }
}
