using BookCollector.Data;

namespace BookCollector.Services.Search
{
    public class SearchResult
    {
        public Book Book { get; private set; }
        public double Score { get; private set; }

        public SearchResult(Book book, double score)
        {
            Book = book;
            Score = score;
        }
    }
}
