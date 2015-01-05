using System;
using Caliburn.Micro;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Model
{
    [JsonObject(MemberSerialization.OptOut)]
    public class CollectionDescription : ReactiveObject, IHaveDisplayName
    {
        [JsonProperty]
        public string Id { get; private set; }

        private string _DisplayName = "None";
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        private string _Info = "Books: 0";
        public string Info
        {
            get { return _Info; }
            set { this.RaiseAndSetIfChanged(ref _Info, value); }
        }

        private DateTime _Created = DateTime.Now;
        public DateTime Created
        {
            get { return _Created; }
            set { this.RaiseAndSetIfChanged(ref _Created, value); }
        }

        private DateTime _LastModified = DateTime.Now;
        public DateTime LastModified
        {
            get { return _LastModified; }
            set { this.RaiseAndSetIfChanged(ref _LastModified, value); }
        }

        public CollectionDescription()
        {
            Id = Guid.NewGuid().ToString().ToUpperInvariant();
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", DisplayName, Id);
        }
    }
}
