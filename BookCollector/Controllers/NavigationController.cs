using System.ComponentModel.Composition;
using BookCollector.Screens.Main;
using BookCollector.Shell;

namespace BookCollector.Controllers
{
    [Export(typeof(INavigationController))]
    public class NavigationController : INavigationController
    {
        private readonly IBookCollectorShell shell;
        private readonly MainViewModel main;

        [ImportingConstructor]
        public NavigationController(IBookCollectorShell shell, MainViewModel main)
        {
            this.shell = shell;
            this.main = main;
        }

        public void NavigateToMain()
        {
            shell.Show(main);
        }
    }
}
