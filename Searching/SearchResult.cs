namespace Searching
{
    public class SearchResult
    {
        public Document Document { get; private set; }
        public double Score { get; private set; }

        public SearchResult(Document document, double score)
        {
            Document = document;
            Score = score;
        }
    }
}
