using System.Windows;

namespace Mockups
{
    public partial class WebView
    {
        public WebView()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.MainWindow as MainWindow;
            window.ShowBooksView();
        }

        private void HomeClick(object sender, RoutedEventArgs e)
        {
            WebBrowser.Address = "http://www.google.com";
        }
    }
}
