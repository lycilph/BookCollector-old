using System.ComponentModel.Composition;
using BookCollector.Screens.Import;
using BookCollector.Screens.Main;
using BookCollector.Screens.Search;
using BookCollector.Screens.Selection;
using BookCollector.Shell;
using Caliburn.Micro;

namespace BookCollector.Controllers
{
    [Export(typeof(INavigationController))]
    public class NavigationController : INavigationController
    {
        private readonly IBookCollectorShell shell;

        [ImportingConstructor]
        public NavigationController(IBookCollectorShell shell)
        {
            this.shell = shell;
        }

        public void Back()
        {
            shell.Back();
        }

        public void NavigateToMain()
        {
            shell.Show(IoC.Get<MainViewModel>());
        }

        public void NavigateToSearch()
        {
            shell.Show(IoC.Get<SearchViewModel>());
        }

        public void NavigateToImport()
        {
            shell.Show(IoC.Get<ImportViewModel>());
        }

        public void NavigateToSelection()
        {
            shell.Show(IoC.Get<SelectionViewModel>());
        }
    }
}
