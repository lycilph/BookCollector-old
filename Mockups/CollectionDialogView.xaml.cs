using System;
using System.Windows;

namespace Mockups
{
    public partial class CollectionDialogView
    {
        private Action close_handler;

        public CollectionDialogView(Action close_handler)
        {
            InitializeComponent();
            this.close_handler = close_handler;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            close_handler();
        }
    }
}
