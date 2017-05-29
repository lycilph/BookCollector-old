using System.Linq;
using BookCollector.Extensions;
using BookCollector.Framework.Logging;
using BookCollector.Framework.MessageBus;
using BookCollector.Models;
using BookCollector.Shell;
using ReactiveUI;

namespace BookCollector.Screens.Start
{
    public class StartViewModel : ShellScreenBase
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IEventAggregator event_aggregator;
        private IBookCollectorModel book_collector_model;

        private ReactiveList<CollectionDescriptionViewModel> _Collections = new ReactiveList<CollectionDescriptionViewModel>();
        public ReactiveList<CollectionDescriptionViewModel> Collections
        {
            get { return _Collections; }
            set { this.RaiseAndSetIfChanged(ref _Collections, value); }
        }

        private CollectionDescriptionViewModel _SelectedCollection;
        public CollectionDescriptionViewModel SelectedCollection
        {
            get { return _SelectedCollection; }
            set { this.RaiseAndSetIfChanged(ref _SelectedCollection, value); }
        }

        private ReactiveCommand _AddCollectionCommand;
        public ReactiveCommand AddCollectionCommand
        {
            get { return _AddCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _AddCollectionCommand, value); }
        }

        private ReactiveCommand _RemoveCollectionCommand;
        public ReactiveCommand RemoveCollectionCommand
        {
            get { return _RemoveCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _RemoveCollectionCommand, value); }
        }

        private ReactiveCommand _ContinueCommand;
        public ReactiveCommand ContinueCommand
        {
            get { return _ContinueCommand; }
            set { this.RaiseAndSetIfChanged(ref _ContinueCommand, value); }
        }

        public StartViewModel(IEventAggregator event_aggregator, IBookCollectorModel book_collector_model)
        {
            log.Info("StartViewModel created");

            this.event_aggregator = event_aggregator;
            this.book_collector_model = book_collector_model;

            DisplayName = ScreenNames.StartScreenName;

            var can_continue = this.WhenAny(x => x.SelectedCollection, c => c.Value != null);
            ContinueCommand = ReactiveCommand.Create(() => Continue(), can_continue);
            
            //AddUserCommand = ReactiveCommand.Create(() => AddUser());

            //var can_remove_user = this.WhenAny(x => x.SelectedUser, user => user.Value != null);
            //RemoveUserCommand = ReactiveCommand.Create(() => RemoveUser(), can_remove_user);
        }

        public override void Activate()
        {
            // Update from model
            Collections = book_collector_model.GetAllCollectionDescriptions()
                                              .Select(c => new CollectionDescriptionViewModel(c))
                                              .ToReactiveList();
            SelectedCollection = Collections.FirstOrDefault();

            //Users = book_collector_model.Users.CreateDerivedCollection(x => new UserViewModel(x));
            //SelectedUser = Users.Single(u => u.Matches(book_collector_model.CurrentUser));
            //SelectedUser.SelectedCollection = SelectedUser.Collections.Single(c => c.Matches(book_collector_model.CurrentCollection));
        }

        private void Continue()
        {
            //// Update model
            //book_collector_model.CurrentUser = SelectedUser.Unwrap();
            //book_collector_model.CurrentCollection = SelectedUser.SelectedCollection.Unwrap();
            //// Navigate to main screen
            //event_aggregator.Publish(new NavigationMessage(ScreenNames.MainScreenName));
        }

        //private void AddUser()
        //{
        //    var new_user = book_collector_model.AddUser();
        //    SelectedUser = Users.Single(u => u.Matches(new_user));
        //}

        //private void RemoveUser()
        //{
        //    var user_to_remove = SelectedUser.Unwrap();
        //    var user_index = Users.Select((user, index) => new { user, index }).First(p => p.user == SelectedUser).index;

        //    if (Users.Count == 1)
        //    {
        //        SelectedUser = null;
        //    }
        //    else
        //    {
        //        if (user_index == 0)
        //            SelectedUser = Users.ElementAt(1);
        //        else
        //            SelectedUser = Users.ElementAt(user_index - 1);
        //    }

        //    book_collector_model.RemoveUser(user_to_remove);
        //}
    }
}
