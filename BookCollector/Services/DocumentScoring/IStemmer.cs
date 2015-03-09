namespace BookCollector.Services.DocumentScoring
{
    public interface IStemmer
    {
        string StemTerm(string s);
    }
}