using System.Windows;

namespace Mockups
{
    public partial class WebView
    {
        public WebView()
        {
            InitializeComponent();
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.MainWindow as MainWindow;
            window.ShowBooksView();
        }
    }
}
