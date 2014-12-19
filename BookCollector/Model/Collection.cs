using System;
using ReactiveUI;

namespace BookCollector.Model
{
    public class Collection : ReactiveObject
    {
        public string Id { get; set; }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        public Collection()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
