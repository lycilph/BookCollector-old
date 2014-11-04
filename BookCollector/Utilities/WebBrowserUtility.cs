using System.Windows;
using System.Windows.Controls;

namespace BookCollector.Utilities
{
    public static class WebBrowserUtility
    {
        public static string GetSource(DependencyObject obj)
        {
            return (string)obj.GetValue(SourceProperty);
        }
        public static void SetSource(DependencyObject obj, string value)
        {
            obj.SetValue(SourceProperty, value);
        }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(string), typeof(WebBrowserUtility), new PropertyMetadata(null, SourceChanged));

        public static void SourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var browser = o as WebBrowser;
            if (browser == null) return;

            var str = e.NewValue as string;
            browser.Navigate(str);
        }

    }
}
