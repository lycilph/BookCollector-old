namespace Searching
{
    public class Shelf
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Locked { get; set; }

        public Shelf(string name, string description = "", bool locked = false)
        {
            Name = name;
            Description = description;
            Locked = locked;
        }
    }
}
