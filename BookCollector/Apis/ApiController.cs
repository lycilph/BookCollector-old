using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Services;
using BookCollector.Utilities;
using ReactiveUI;

namespace BookCollector.Apis
{
    [Export(typeof(ApiController))]
    public class ApiController : ReactiveObject
    {
        private readonly ApplicationSettings application_settings;

        private ReactiveList<IApi> _Apis;
        public ReactiveList<IApi> Apis
        {
            get { return _Apis; }
            set { this.RaiseAndSetIfChanged(ref _Apis, value); }
        }

        private ReactiveList<IImportController> _ImportControllers;
        public ReactiveList<IImportController> ImportControllers
        {
            get { return _ImportControllers; }
            set { this.RaiseAndSetIfChanged(ref _ImportControllers, value); }
        }

        [ImportingConstructor]
        public ApiController(ApplicationSettings application_settings, [ImportMany] IEnumerable<IApi> apis, [ImportMany] IEnumerable<IImportController> import_controllers)
        {
            this.application_settings = application_settings;
            Apis = apis.ToReactiveList();
            ImportControllers = import_controllers.ToReactiveList();
        }

        public List<IImportController> GetImportControllers()
        {
            return ImportControllers.Where(i => application_settings.IsApiEnabled(i.ApiName)).ToList();
        }
    }
}
