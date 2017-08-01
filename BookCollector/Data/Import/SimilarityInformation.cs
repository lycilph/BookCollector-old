namespace BookCollector.Data.Import
{
    public class SimilarityInformation
    {
        public Book Book { get; set; }
        public int Score { get; set; } = 0;
        public string Text { get; set; } = string.Empty;
        public string TextShort { get; set; } = string.Empty;

        public SimilarityInformation(Book book)
        {
            Book = book;
        }

        public void Add(int score, string text, string text_short)
        {
            Score += score;
            Text += ", " + text;
            TextShort += ", " + text_short;
        }

        public void Cleanup()
        {
            Text = Text.TrimStart(new char[] { ',', ' ' });
            TextShort = TextShort.TrimStart(new char[] { ',', ' ' });
        }
    }
}
