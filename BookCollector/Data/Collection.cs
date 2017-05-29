using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Collection : ReactiveObject
    {
        private CollectionDescription _Description = new CollectionDescription();
        public CollectionDescription Description
        {
            get { return _Description; }
            set { this.RaiseAndSetIfChanged(ref _Description, value); }
        }

        private ReactiveList<Book> _Books = new ReactiveList<Book>();
        public ReactiveList<Book> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }
    }
}
