using System.IO;
using DesktopOrganizer.Utils;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    public class BookCollection : ReactiveObject
    {
        private ReactiveList<Book> _Books = new ReactiveList<Book>();
        [JsonProperty]
        public ReactiveList<Book> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        public static BookCollection Load(string filename)
        {
            var result = new BookCollection();

            if (File.Exists(filename))
            {
                var json = File.ReadAllText(filename);
                var temp = JsonConvert.DeserializeObject<BookCollection>(json);
                result.Books = temp.Books.ToReactiveList();
            }

            return result;
        }

        public void Save(string filename)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filename, json);
        }
    }
}
