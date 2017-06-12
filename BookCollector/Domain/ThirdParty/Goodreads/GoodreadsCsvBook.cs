namespace BookCollector.Domain.ThirdParty.Goodreads
{
    public class GoodreadsCsvBook
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string AdditionalAuthors { get; set; }
        public string ISBN { get; set; }
        public string ISBN13 { get; set; }
        public string Bookshelves { get; set; }
        public string ExclusiveShelf { get; set; }
        public string Shelves
        {
            get
            {
                var shelves = string.Empty;
                if (!string.IsNullOrWhiteSpace(Bookshelves))
                    shelves = Bookshelves;
                if (!string.IsNullOrWhiteSpace(ExclusiveShelf))
                    shelves += ", " + ExclusiveShelf;
                return shelves;
            }
        }
        public string Source { get { return "Goodreads CSV"; } }
    }
}
