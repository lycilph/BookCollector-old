namespace BookCollector.ThirdParty.Goodreads
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
        public string Shelf
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Bookshelves))
                    return Bookshelves;
                if (!string.IsNullOrWhiteSpace(ExclusiveShelf))
                    return ExclusiveShelf;
                return string.Empty;
            }
        }
        public string Source { get { return "Goodreads CSV"; } }
    }
}
