using System;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Description : ReactiveObject
    {
        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private string _Summary = string.Empty;
        public string Summary
        {
            get { return _Summary; }
            set { this.RaiseAndSetIfChanged(ref _Summary, value); }
        }

        private DateTime _LastModifiedDate = DateTime.Now;
        public DateTime LastModifiedDate
        {
            get { return _LastModifiedDate; }
            set { this.RaiseAndSetIfChanged(ref _LastModifiedDate, value); }
        }

        private string _Filename = string.Empty;
        [JsonIgnore]
        public string Filename
        {
            get { return _Filename; }
            set { this.RaiseAndSetIfChanged(ref _Filename, value); }
        }
    }
}
