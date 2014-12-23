using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Services.Collections;
using BookCollector.Shell;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Framework.Core.Dialogs;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using ReactiveUI;
using IScreen = Caliburn.Micro.IScreen;
using LogManager = NLog.LogManager;

namespace BookCollector.Screens.Collections
{
    [Export("Collections", typeof(IScreen))]
    public class CollectionsViewModel : ReactiveScreen
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator event_aggregator;
        private readonly CollectionsController collections_controller;

        private CollectionDescriptionViewModel _CurrentCollection;
        public CollectionDescriptionViewModel CurrentCollection
        {
            get { return _CurrentCollection; }
            set { this.RaiseAndSetIfChanged(ref _CurrentCollection, value); }
        }

        private IReactiveDerivedList<CollectionDescriptionViewModel> _Collections;
        public IReactiveDerivedList<CollectionDescriptionViewModel> Collections
        {
            get { return _Collections; }
            set { this.RaiseAndSetIfChanged(ref _Collections, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _CanOk;
        public bool CanOk { get { return _CanOk.Value; }}

        private readonly ObservableAsPropertyHelper<bool> _CanCancel;
        public bool CanCancel { get { return _CanCancel.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanRemove;
        public bool CanRemove { get { return _CanRemove.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanClear;
        public bool CanClear { get { return _CanClear.Value; } }
            
        [ImportingConstructor]
        public CollectionsViewModel(CollectionsController collections_controller, IEventAggregator event_aggregator)
        {
            this.collections_controller = collections_controller;
            this.event_aggregator = event_aggregator;

            _CanOk = this.WhenAny(x => x.CurrentCollection, x => x.Value != null)
                         .ToProperty(this, x => x.CanOk);

            _CanCancel = collections_controller.WhenAny(x => x.Current, x => x.Value != null)
                                               .ToProperty(this, x => x.CanCancel);

            _CanRemove = this.WhenAny(x => x.CurrentCollection, x => x.Value != null)
                             .ToProperty(this, x => x.CanRemove);

            _CanClear = this.WhenAny(x => x.CurrentCollection, x => x.Value != null)
                            .ToProperty(this, x => x.CanClear);

            Collections = collections_controller.Collections.CreateDerivedCollection(c => new CollectionDescriptionViewModel(c));
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            logger.Trace("Activating");
            CurrentCollection = Collections.FirstOrDefault(c => c.AssociatedObject == collections_controller.Current);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            logger.Trace("Deactivating");
            collections_controller.Save();
        }

        public void Add()
        {
            var collection = collections_controller.Create();
            CurrentCollection = Collections.FirstOrDefault(c => c.AssociatedObject == collection);
        }

        public void Remove()
        {
            collections_controller.Remove(CurrentCollection.AssociatedObject);
            CurrentCollection = null;
        }

        public void Clear()
        {
        }

        public void Ok()
        {
            if (CurrentCollection != null)
                collections_controller.Current = CurrentCollection.AssociatedObject;

            Cancel();
        }

        public void Cancel()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.Back());
        }
    }
}
