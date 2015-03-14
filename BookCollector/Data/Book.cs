using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Book : ReactiveObject
    {
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { this.RaiseAndSetIfChanged(ref _Title, value); }
        }

        private ReactiveList<string> _Authors = new ReactiveList<string>();
        public ReactiveList<string> Authors
        {
            get { return _Authors; }
            set { this.RaiseAndSetIfChanged(ref _Authors, value); }
        }

        private ReactiveList<string> _Narrators = new ReactiveList<string>();
        public ReactiveList<string> Narrators
        {
            get { return _Narrators; }
            set { this.RaiseAndSetIfChanged(ref _Narrators, value); }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { this.RaiseAndSetIfChanged(ref _Description, value); }
        }

        private string _Asin;
        public string Asin
        {
            get { return _Asin; }
            set { this.RaiseAndSetIfChanged(ref _Asin, value); }
        }

        private string _ISBN10;
        public string ISBN10
        {
            get { return _ISBN10; }
            set { this.RaiseAndSetIfChanged(ref _ISBN10, value); }
        }

        private string _ISBN13;
        public string ISBN13
        {
            get { return _ISBN13; }
            set { this.RaiseAndSetIfChanged(ref _ISBN13, value); }
        }

        private string _Source;
        public string Source
        {
            get { return _Source; }
            set { this.RaiseAndSetIfChanged(ref _Source, value); }
        }

        private ReactiveList<string> _History = new ReactiveList<string>();
        public ReactiveList<string> History
        {
            get { return _History; }
            set { this.RaiseAndSetIfChanged(ref _History, value); }
        }
    }
}
