using System.Reflection;
using System.Windows;
using System.Windows.Navigation;
using Caliburn.Micro;

namespace BookCollector.Old
{
    public partial class ImportAuthenticationStepView
    {
        public ImportAuthenticationStepView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routed_event_args)
        {
            Loaded -= OnLoaded;

            var field = Browser.GetType().GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null) return;

            var obj = field.GetValue(Browser);
            obj.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, obj, new object[] { true });
        }

        private void Browser_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            dynamic doc = Browser.Document;
            var html = doc.documentElement.InnerHtml;

            Action.Invoke(DataContext, "Loaded", this, this, e, new object[]{html});
        }
    }
}
