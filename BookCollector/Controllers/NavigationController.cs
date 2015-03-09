using System.ComponentModel.Composition;
using BookCollector.Screens.Main;
using BookCollector.Screens.Search;
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

        public void NavigateToMain()
        {
            shell.Show(IoC.Get<MainViewModel>());
        }

        public void NavigateToSearch()
        {
            shell.Show(IoC.Get<SearchViewModel>());
        }
    }
}
