using System;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    public class Info : ReactiveObject
    {
        private DateTime _Created = DateTime.Now;
        [JsonProperty]
        public DateTime Created
        {
            get { return _Created; }
            set { this.RaiseAndSetIfChanged(ref _Created, value); }
        }

        private DateTime _LastUpdated = DateTime.Now;
        [JsonProperty]
        public DateTime LastUpdated
        {
            get { return _LastUpdated; }
            set { this.RaiseAndSetIfChanged(ref _LastUpdated, value); }
        }

        private string _DisplayName = string.Empty;
        [JsonProperty]
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        private string _Filename = string.Empty;
        [JsonProperty]
        public string Filename
        {
            get { return _Filename; }
            set { this.RaiseAndSetIfChanged(ref _Filename, value); }
        }

        private string _Text = string.Empty;
        [JsonProperty]
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }

        public BookCollection Load()
        {
            return BookCollection.Load(Filename);
        }

        public void Save(BookCollection collection)
        {
            collection.Save(Filename);
        }
    }
}
