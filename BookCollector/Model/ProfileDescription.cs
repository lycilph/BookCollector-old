using System;
using Caliburn.Micro;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Model
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ProfileDescription : ReactiveObject, IHaveDisplayName
    {
        [JsonProperty]
        public string Id { get; private set; }

        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        private ReactiveList<CollectionDescription> _Collections = new ReactiveList<CollectionDescription>();
        public ReactiveList<CollectionDescription> Collections
        {
            get { return _Collections; }
            set { this.RaiseAndSetIfChanged(ref _Collections, value); }
        }

        public ProfileDescription()
        {
            Id = Guid.NewGuid().ToString().ToUpperInvariant();
        }

        public void Add(CollectionDescription collection)
        {
            collection.Profile = this;
            Collections.Add(collection);
        }

        public void Remove(CollectionDescription collection)
        {
            collection.Profile = null;
            Collections.Remove(collection);
        }
    }
}
