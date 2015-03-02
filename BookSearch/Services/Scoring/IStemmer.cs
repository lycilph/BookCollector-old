namespace BookSearch.Services.Scoring
{
    public interface IStemmer
    {
        string StemTerm(string s);
    }
}