using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Apis.Amazon;
using BookCollector.Controllers;
using BookCollector.Utilities;
using ReactiveUI;

namespace BookCollector.Screens.MissingImages
{
    [Export("MissingImages", typeof(IShellScreen))]
    public class MissingImagesViewModel : ShellScreenBase
    {
        private readonly ApplicationController application_controller;
        private readonly AmazonApi api;

        private ReactiveList<MissingImagesBookViewModel> _Books;
        public ReactiveList<MissingImagesBookViewModel> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        [ImportingConstructor]
        public MissingImagesViewModel(ApplicationController application_controller, AmazonApi api)
        {
            this.application_controller = application_controller;
            this.api = api;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Books = application_controller.BookRepository.Books.Where(b => !b.HasImages()).Select(b => new MissingImagesBookViewModel(b)).ToReactiveList();
        }

        public void Back()
        {
            application_controller.NavigateBack();
        }
    }
}
