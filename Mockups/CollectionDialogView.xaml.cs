using System;
using System.Windows.Controls;

namespace Mockups
{
    public partial class CollectionDialogView : UserControl
    {
        private Action close_handler;

        public CollectionDialogView(Action close_handler)
        {
            InitializeComponent();
            this.close_handler = close_handler;
        }

        private void CloseClick(object sender, System.Windows.RoutedEventArgs e)
        {
            close_handler();
        }
    }
}
