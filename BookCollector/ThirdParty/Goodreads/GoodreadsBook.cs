namespace BookCollector.ThirdParty.Goodreads
{
    public class GoodreadsBook
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string AdditionalAuthors { get; set; }
        public string ISBN { get; set; }
        public string ISBN13 { get; set; }
        public string Bookshelves { get; set; }
        public string ExclusiveShelf { get; set; }
        public string Source { get { return "Goodreads CSV"; } }
    }
}
