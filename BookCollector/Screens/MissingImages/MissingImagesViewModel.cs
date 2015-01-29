using System.ComponentModel.Composition;
using BookCollector.Apis.Amazon;
using BookCollector.Controllers;

namespace BookCollector.Screens.MissingImages
{
    [Export("MissingImages", typeof(IShellScreen))]
    public class MissingImagesViewModel : ShellScreenBase
    {
        private readonly ApplicationController application_controller;
        private readonly AmazonApi api;

        [ImportingConstructor]
        public MissingImagesViewModel(ApplicationController application_controller, AmazonApi api)
        {
            this.application_controller = application_controller;
            this.api = api;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            api.Search("1857237471");
        }

        public void Back()
        {
            application_controller.NavigateBack();
        }
    }
}
