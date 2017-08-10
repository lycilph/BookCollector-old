using System;
using ReactiveUI;

namespace Core.Shell
{
    public class WindowCommand : ReactiveObject, IWindowCommand
    {
        private readonly Action action;

        private object _DisplayName;
        public object DisplayName
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

        public ReactiveCommand ExecuteCommand { get; private set; }

        public WindowCommand(object name, Action action)
        {
            this.action = action;
            DisplayName = name;

            ExecuteCommand = ReactiveCommand.Create(action);
        }
    }
}
