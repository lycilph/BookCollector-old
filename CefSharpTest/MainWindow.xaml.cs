using System;
using System.Diagnostics;
using System.Windows;
using CefSharp;

namespace CefSharpTest
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            //browser.FrameLoadStart += BrowserOnFrameLoadStart;
            browser.FrameLoadEnd += BrowserOnFrameLoadEnd;
        }

        private void BrowserOnFrameLoadEnd(object sender, FrameLoadEndEventArgs args)
        {
            if (args.IsMainFrame)
                Debug.WriteLine("Frame load end [{0}]: {1}", args.HttpStatusCode, args.Url);
            //Debug.WriteLine("Frame load end [{0}, {1}]: {2}", args.HttpStatusCode, args.IsMainFrame, args.Url);
        }

        private void BrowserOnFrameLoadStart(object sender, FrameLoadStartEventArgs args)
        {
            Debug.WriteLine("Frame load start [{0}]: {1}", args.IsMainFrame, args.Url);
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            browser.Address = @"http://www.audible.com";
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //var script = String.Format("document.getElementById(\"adbl_time_filter\").selectedIndex = 5; document.getElementById(\"adbl_time_filter\").change()");
            //browser.ExecuteScriptAsync(script);
        }

        private void ReloadOnClick(object sender, RoutedEventArgs e)
        {
            //browser.Reload();
        }

        private async void GetOnClick(object sender, RoutedEventArgs e)
        {
            //var txt = await browser.GetSourceAsync();
        }
    }
}
