using System.Windows.Controls;

namespace Mockups
{
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void ShelfChangedClick(object sender, SelectionChangedEventArgs e)
        {
            if (MenuToggleButton != null)
                MenuToggleButton.IsChecked = !MenuToggleButton.IsChecked;
        }
    }
}
