namespace BookCollector.Data
{
    public class Source
    {
        public string Import { get; set; }
        public string Image { get; set; }
        public string Information { get; set; }

        public Source(string import)
        {
            Import = import;
        }
    }
}
