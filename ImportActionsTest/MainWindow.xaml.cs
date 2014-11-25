using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ImportActionsTest
{
    public partial class MainWindow
    {
        public List<IImportAction> Actions { get; set; }
        public IImportAction CurrentAction { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Actions = new List<IImportAction> {new SelectApiAction()};
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            var import_action = Actions.First();
        }
    }
}
