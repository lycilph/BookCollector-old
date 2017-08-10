using System.Windows.Input;

namespace BookCollector.Screens.Import
{
    public partial class AddShelfDialogView
    {
        public AddShelfDialogView()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Keyboard.Focus(NameTextBox);
            NameTextBox.CaretIndex = NameTextBox.Text.Length;
        }
    }
}
