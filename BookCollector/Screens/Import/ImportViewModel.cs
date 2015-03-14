using System.ComponentModel.Composition;
using BookCollector.Controllers;
using BookCollector.Services.GoodreadsCsv;

namespace BookCollector.Screens.Import
{
    [Export(typeof(ImportViewModel))]
    public class ImportViewModel : BookCollectorScreenBase
    {
        private readonly INavigationController navigation_controller;
        private readonly IDataController data_controller;

        [ImportingConstructor]
        public ImportViewModel(INavigationController navigation_controller, IDataController data_controller)
        {
            this.navigation_controller = navigation_controller;
            this.data_controller = data_controller;
        }

        public void Back()
        {
            navigation_controller.Back();
        }

        public void Import()
        {
            const string path = @"C:\Private\Projects\BookCollector\Data\goodreads_export.csv";
            var books = Importer.Read(path);
            data_controller.Collection.AddRange(books);
            Back();
        }
    }
}
