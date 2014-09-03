using System.ComponentModel.Composition;
using Framework.Core;
using Framework.Dialogs;
using Framework.MainMenu.ViewModels;
using Framework.Module;
using Framework.Window;

namespace BookCollector.About
{
    [Export(typeof(IModule))]
    [ExportMetadata("Order", 3)]
    public class AboutModule : ModuleBase
    {
        public override void Create(IShell shell)
        {
            shell.RightShellCommands.Add(new WindowCommand("About", ShowAbout));
            shell.Menu.Add(new MenuItem("_Help")
            {
                new MenuItem("_About", ShowAbout),
            });
        }

        public async void ShowAbout()
        {
            await DialogController.ShowAsync(new AboutViewModel(), DialogButtonOptions.Ok);
        }
    }
}
