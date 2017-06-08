using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Collection : ReactiveObject
    {
        private Description _Description = new Description();
        public Description Description
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

        public bool Matches(Description description)
        {
            return Description.Equals(description);
        }
    }
}
