using System.ComponentModel.Composition;
using System.Linq;
using System.Xml.Linq;
using BookCollector.Controllers;
using BookCollector.Data;
using MahApps.Metro.Controls.Dialogs;
using Panda.ApplicationCore.Dialogs;
using Panda.ApplicationCore.Utilities;
using ReactiveUI;

namespace BookCollector.Screens.Selection
{
    [Export(typeof(SelectionViewModel))]
    public class SelectionViewModel : BookCollectorScreenBase
    {
        private readonly IDataController data_controller;
        private readonly INavigationController navigation_controller;

        public override bool ShowCurrentUser { get { return false; } }

        private UserViewModel _CurrentUser;
        public UserViewModel CurrentUser
        {
            get { return _CurrentUser; }
            set { this.RaiseAndSetIfChanged(ref _CurrentUser, value); }
        }

        private ReactiveList<UserViewModel> _Users;
        public ReactiveList<UserViewModel> Users
        {
            get { return _Users; }
            set { this.RaiseAndSetIfChanged(ref _Users, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _CanRemoveUser;
        public bool CanRemoveUser { get { return _CanRemoveUser.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanAddCollection;
        public bool CanAddCollection { get { return _CanAddCollection.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanRemoveCollection;
        public bool CanRemoveCollection { get { return _CanRemoveCollection.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanOk;
        public bool CanOk { get { return _CanOk.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanCancel;
        public bool CanCancel { get { return _CanCancel.Value; } }

        [ImportingConstructor]
        public SelectionViewModel(IDataController data_controller, INavigationController navigation_controller)
        {
            this.data_controller = data_controller;
            this.navigation_controller = navigation_controller;

            var is_user_not_null = this.WhenAny(x => x.CurrentUser, x => x.Value != null);
            _CanRemoveUser = is_user_not_null.ToProperty(this, x => x.CanRemoveUser);
            _CanAddCollection = is_user_not_null.ToProperty(this, x => x.CanAddCollection);

            var is_user_and_collection_not_null = this.WhenAny(x => x.CurrentUser, x => x.CurrentUser.CurrentCollection, (u, c) => u.Value != null && c.Value != null);
            _CanRemoveCollection = is_user_and_collection_not_null.ToProperty(this, x => x.CanRemoveCollection);
            _CanOk = is_user_and_collection_not_null.ToProperty(this, x => x.CanOk);

            _CanCancel = data_controller.WhenAny(x => x.User, x => x.Value != null).ToProperty(this, x => x.CanCancel);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Users = data_controller.GetAllUsers()
                                   .Select(u => new UserViewModel(u))
                                   .ToReactiveList();

            if (Users != null && Users.Any())
            {
                CurrentUser = Users.FirstOrDefault(vm => vm.AssociatedObject.Id == data_controller.User.Id);

                if (CurrentUser != null)
                    CurrentUser.CurrentCollection = CurrentUser.Collections.FirstOrDefault(vm => vm.AssociatedObject.Id == data_controller.Collection.Id);
            }
        }

        public void Ok()
        {
            data_controller.Save(Users.Select(u => u.AssociatedObject).ToList());
            data_controller.User = CurrentUser.AssociatedObject;
            data_controller.Collection = CurrentUser.CurrentCollection.AssociatedObject;

            navigation_controller.Back();
        }

        public void Cancel()
        {
            navigation_controller.Back();
        }

        public void AddUser()
        {
            Users.Add(new UserViewModel(new User()));
        }

        public async void RemoveUser()
        {
            var msg = string.Format("This will delete the user {0} and all collections belonging to that user", CurrentUser.Name);
            var answer = await DialogController.ShowMessageAsync("Deleting user", msg, MessageDialogStyle.AffirmativeAndNegative);
            if (answer == MessageDialogResult.Affirmative)
            {
                data_controller.Delete(CurrentUser.AssociatedObject);
                Users.Remove(CurrentUser);
                CurrentUser = null;
            }
        }

        public void AddCollection()
        { }

        public void RemoveCollection()
        { }
    }
}
