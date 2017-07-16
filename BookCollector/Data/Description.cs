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

        private string _Text = string.Empty;
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }

        private DateTime _LastModifiedDate;
        public DateTime LastModifiedDate
        {
            get { return _LastModifiedDate; }
            set { this.RaiseAndSetIfChanged(ref _LastModifiedDate, value); }
        }

        [JsonIgnore]
        public string Filename { get; set; }
        [JsonIgnore]
        public int BooksCount { get; set; }
        [JsonIgnore]
        public int ShelfCount { get; set; }

        public Description() { }
        public Description(string name, string text)
        {
            Name = name;
            Text = text;
        }
    }
}
