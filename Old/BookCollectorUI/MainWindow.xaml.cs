using System.Windows;
using System.Windows.Navigation;

namespace BookCollectorUI
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Browser.Navigating += BrowserOnNavigating;
            Browser.Navigate("http://www.audible.com/lib");
        }

        private void BrowserOnNavigating(object sender, NavigatingCancelEventArgs navigating_cancel_event_args)
        {
        }

        private void OnDebugClick(object sender, RoutedEventArgs e)
        {
            dynamic doc = Browser.Document;
            var htmlText = doc.documentElement.InnerHtml;

            var he = doc.GetElementById("adbl_time_filter");
            he.selectedIndex = 5;
            he.onchange();
        }
    }
}
