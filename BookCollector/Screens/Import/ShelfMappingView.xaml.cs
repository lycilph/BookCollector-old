using System.Windows;
using System.Windows.Input;

namespace BookCollector.Screens.Import
{
    public partial class ShelfMappingView
    {
        public ShelfMappingView()
        {
            InitializeComponent();
        }

        private void TextBoxIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (shelf_name_textbox.IsVisible)
            {
                Keyboard.Focus(shelf_name_textbox);
                shelf_name_textbox.CaretIndex = shelf_name_textbox.Text.Length;
            }
            else
                Keyboard.Focus(shelf_combobox);
        }
    }
}
