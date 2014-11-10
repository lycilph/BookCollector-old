using System.Reflection;
using System.Windows;

namespace BookCollector.Import
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
    }
}
