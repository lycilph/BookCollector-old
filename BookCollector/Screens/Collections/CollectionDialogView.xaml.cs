using System.Windows;
using System.Windows.Input;

namespace BookCollector.Screens.Collections
{
    public partial class CollectionDialogView
    {
        public CollectionDialogView()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(NameTextBox);
        }
    }
}
