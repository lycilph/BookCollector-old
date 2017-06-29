using ReactiveUI;

namespace BookCollector.Data
{
    public class Description : ReactiveObject
    {
        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private string _Text = string.Empty;
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }

        public Description() { }
        public Description(string name, string text)
        {
            Name = name;
            Text = text;
        }
    }
}
