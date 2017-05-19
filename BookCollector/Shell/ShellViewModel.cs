using BookCollector.Screens.Main;
using Caliburn.Micro;
using ReactiveUI;

namespace BookCollector.Shell
{
    public class ShellViewModel : ReactiveObject, IHaveDisplayName
    {
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { this.RaiseAndSetIfChanged(ref _IsEnabled, value); }
        }

        private ReactiveList<IWindowCommand> _RightShellCommands = new ReactiveList<IWindowCommand>();
        public ReactiveList<IWindowCommand> RightShellCommands
        {
            get { return _RightShellCommands; }
            set { this.RaiseAndSetIfChanged(ref _RightShellCommands, value); }
        }

        private ReactiveObject _ShellContent;
        public ReactiveObject ShellContent
        {
            get { return _ShellContent; }
            set { this.RaiseAndSetIfChanged(ref _ShellContent, value); }
        }

        public ShellViewModel(MainViewModel main_view_model)
        {
            DisplayName = "Book Collector";
            IsEnabled = true;
            ShellContent = main_view_model;

            RightShellCommands.Add(new WindowCommand("Tools", () => System.Windows.MessageBox.Show("You pressed tools")));
        }
    }
}
