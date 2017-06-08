using System;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Description : ReactiveObject, IEquatable<Description>
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }

        private DateTime _LastModfied;
        public DateTime LastModfied
        {
            get { return _LastModfied; }
            set { this.RaiseAndSetIfChanged(ref _LastModfied, value); }
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

        public bool Equals(Description other)
        {
            return Name.Equals(other.Name) &&
                   Text.Equals(other.Text) &&
                   LastModfied.Equals(other.LastModfied);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Description)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = 269;
                hashcode = (hashcode * 47) + (!string.IsNullOrEmpty(Name) ? Name.GetHashCode() : 0);
                hashcode = (hashcode * 47) + (!string.IsNullOrEmpty(Text) ? Text.GetHashCode() : 0);
                hashcode = (hashcode * 47) + (LastModfied != null ? LastModfied.GetHashCode() : 0);
                return hashcode;
            }
        }
    }
}
