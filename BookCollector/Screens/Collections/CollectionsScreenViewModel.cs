using System;
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

        private ReactiveCommand _ContinueCommand;
        public ReactiveCommand ContinueCommand
        {
            get { return _ContinueCommand; }
            set { this.RaiseAndSetIfChanged(ref _ContinueCommand, value); }
        }

        private ReactiveCommand _CancelCommand;
        public ReactiveCommand CancelCommand
        {
            get { return _CancelCommand; }
            set { this.RaiseAndSetIfChanged(ref _CancelCommand, value); }
        }

        public CollectionsScreenViewModel(ICollectionsService collections_service)
        {
            DisplayName = "Collections";
            this.collections_service = collections_service;

            Initialize();
        }

        private void Initialize()
        {
        }

        public override void Activate()
        {
            base.Activate();

            // Load collection descriptions here
            Descriptions = collections_service.GetAllCollections()
                                              .Select(c => new DescriptionViewModel(c))
                                              .ToReactiveList();

            SelectedDescription = Descriptions.FirstOrDefault();
        }
    }
}
