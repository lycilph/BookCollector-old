﻿using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    public class Book : ReactiveObject
    {
        private BookStatus _Status = BookStatus.Invalid;
        [JsonProperty]
        public BookStatus Status
        {
            get { return _Status; }
            set { this.RaiseAndSetIfChanged(ref _Status, value); }
        }

        private string _GoodreadsId = string.Empty;
        [JsonProperty]
        public string GoodreadsId
        {
            get { return _GoodreadsId; }
            set { this.RaiseAndSetIfChanged(ref _GoodreadsId, value); }
        }

        private string _Title = string.Empty;
        [JsonProperty]
        public string Title
        {
            get { return _Title; }
            set { this.RaiseAndSetIfChanged(ref _Title, value); }
        }

        private string _Author = string.Empty;
        [JsonProperty]
        public string Author
        {
            get { return _Author; }
            set { this.RaiseAndSetIfChanged(ref _Author, value); }
        }

        private string _ISBN = string.Empty;
        [JsonProperty]
        public string ISBN
        {
            get { return _ISBN; }
            set { this.RaiseAndSetIfChanged(ref _ISBN, value); }
        }

        private string _ISBN13 = string.Empty;
        [JsonProperty]
        public string ISBN13
        {
            get { return _ISBN13; }
            set { this.RaiseAndSetIfChanged(ref _ISBN13, value); }
        }

        private string _Image = string.Empty;
        [JsonProperty]
        public string Image
        {
            get { return _Image; }
            set { this.RaiseAndSetIfChanged(ref _Image, value); }
        }
    }
}
