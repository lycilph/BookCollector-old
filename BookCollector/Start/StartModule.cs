using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Shell;
using Caliburn.Micro;
using Framework.Core;
using Framework.MainMenu.ViewModels;
using Framework.Module;

namespace BookCollector.Start
{
    [Export(typeof(IModule))]
    [ExportMetadata("Order", 2)]
    public class StartModule : ModuleBase
    {
        private readonly IEventAggregator event_aggregator;
        private readonly IStartTool start;

        [ImportingConstructor]
        public StartModule(IStartTool start, IEventAggregator event_aggregator)
        {
            this.start = start;
            this.event_aggregator = event_aggregator;
        }

        public override void Create(IShell shell)
        {
            base.Create(shell);

            var file_menu = shell.Menu.Single(m => m.Name.ToLowerInvariant() == "file");
            file_menu.Children.Insert(0, new MenuItem("_Open", OpenCollection));
            file_menu.Children.Insert(0, new MenuItem("_New", NewCollection));

            shell.Menu.Add(new MenuItem("_Window")
            {
                new MenuItem("_Start Page", () => event_aggregator.PublishOnCurrentThread(ShellMessage.Show(start))),
            });
        }

        public override void Initialize()
        {
            base.Initialize();
            event_aggregator.PublishOnCurrentThread(ShellMessage.Show(start));
        }

        private void NewCollection()
        {
            start.NewCollection();
        }

        private void OpenCollection()
        {
            start.NewCollection();
        }
    }
}
