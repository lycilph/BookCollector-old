using System.Linq;
using BookCollector.Services;
using Core.Extensions;
using Core.Shell;
using NLog;
using ReactiveUI;

namespace BookCollector.Screens.Collections
{
    public class CollectionsScreenViewModel : ScreenBase, ICollectionsScreen
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private ICollectionsService collections_service;

        private ReactiveList<DescriptionViewModel> _Descriptions;
        public ReactiveList<DescriptionViewModel> Descriptions
        {
            get { return _Descriptions; }
            set { this.RaiseAndSetIfChanged(ref _Descriptions, value); }
        }

        private DescriptionViewModel _SelectedDescription;
        public DescriptionViewModel SelectedDescription
        {
            get { return _SelectedDescription; }
            set { this.RaiseAndSetIfChanged(ref _SelectedDescription, value); }
        }

        public CollectionsScreenViewModel(ICollectionsService collections_service)
        {
            DisplayName = "Collections";
            this.collections_service = collections_service;
        }

        public override void Activate()
        {
            base.Activate();

            // Load collection descriptions here
            Descriptions = collections_service.GetCollectionDescriptions()
                                              .Select(c => new DescriptionViewModel(c))
                                              .ToReactiveList();

            SelectedDescription = Descriptions.FirstOrDefault();
        }
    }
}
