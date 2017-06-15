using System.Windows.Input;

namespace BookCollector.Views.Dialogs
{
    public partial class DescriptionDialogView
    {
        public DescriptionDialogView()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Keyboard.Focus(NameTextBox);
        }
    }
}
