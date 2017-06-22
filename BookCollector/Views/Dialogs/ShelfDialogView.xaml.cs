using System.Windows;
using System.Windows.Input;

namespace BookCollector.Views.Dialogs
{
    public partial class ShelfDialogView
    {
        public ShelfDialogView()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(NameTextBox);
        }
    }
}
