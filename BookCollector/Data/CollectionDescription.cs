using System;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class CollectionDescription : ReactiveObject
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private DateTime _LastModfied;
        public DateTime LastModfied
        {
            get { return _LastModfied; }
            set { this.RaiseAndSetIfChanged(ref _LastModfied, value); }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { this.RaiseAndSetIfChanged(ref _Description, value); }
        }

        // Other data
        // - Checksum (used for change tracking, see panda framework)

        private string _Filename;
        [JsonIgnore]
        public string Filename
        {
            get { return _Filename; }
            set { this.RaiseAndSetIfChanged(ref _Filename, value); }
        }

        public override bool Equals(object obj)
        {
            var other = obj as CollectionDescription;
            if (other == null)
                return false;

            return Name == other.Name &&
                   LastModfied == other.LastModfied &&
                   Description == other.Description;
        }

        public override int GetHashCode()
        {
            return new { Name, LastModfied, Description }.GetHashCode();
        }
    }
}
