using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using BookCollector.Data;
using BookCollector.Services;
using Panda.Utilities.Extensions;
using ReactiveUI;

namespace BookCollector.Controllers
{
    [Export(typeof(IDataController))]
    public class DataController : ReactiveObject, IDataController
    {
        private const string UserExtension = ".user";

        private readonly ISettings settings;

        private User _User;
        public User User
        {
            get { return _User; }
            set
            {
                this.RaiseAndSetIfChanged(ref _User, value);
                settings.LastUserId = (_User == null ? string.Empty : _User.Id);
            }
        }

        private Collection _Collection;
        public Collection Collection
        {
            get { return _Collection; }
            set
            {
                this.RaiseAndSetIfChanged(ref _Collection, value);
                settings.LastCollectionId = (_Collection == null ? string.Empty : _Collection.Id);
            }
        }

        [ImportingConstructor]
        public DataController(ISettings settings)
        {
            this.settings = settings;
        }

        private void CreateNewUserAndCollection()
        {
            User = new User();
            Collection = new Collection();
            User.Add(Collection);
        }

        public void Load()
        {
            if (!settings.LoadLastCollection || string.IsNullOrWhiteSpace(settings.LastUserId))
            {
                CreateNewUserAndCollection();
                return;
            }

            var user_path = settings.GetPathFor(settings.LastUserId + UserExtension);
            if (!File.Exists(user_path))
            {
                CreateNewUserAndCollection();
                return;
            }

            User = JsonExtensions.ReadFromFile<User>(user_path);
            User.IsDirty = false;

            Collection = User.Collections.FirstOrDefault(c => c.Id == settings.LastCollectionId) ?? User.Collections.First();
        }

        public void Save()
        {
            Save(User);
        }

        public void Save(List<User> users)
        {
            users.Where(u => u.IsDirty).Apply(Save);
        }

        private void Save(User user)
        {
            if (user == null || !user.IsDirty)
                return;

            var user_path = settings.GetPathFor(user.Id + UserExtension);
            JsonExtensions.WriteToFile(user_path, user);
        }

        public void Delete(User user)
        {
            if (user.Id == User.Id)
                User = null;

            var user_path = settings.GetPathFor(user.Id + UserExtension);
            File.Delete(user_path);
        }

        public List<User> GetAllUsers()
        {
            return Directory.EnumerateFiles(settings.DataFolder, "*" + UserExtension) // Find all user files in data folder
                            .Select(JsonExtensions.ReadFromFile<User>)                // Read all saved users
                            .Concat(new List<User> { User })                          // Add current user (if it has not been saved yet, we need to add it here)
                            .DistinctBy(u => u.Id)                                    // Make sure current user is not present twice
                            .ToList();
        }
    }
}
