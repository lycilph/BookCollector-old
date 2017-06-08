using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Collection : ReactiveObject
    {
        private Description _Description;
        public Description Description
        {
            get { return _Description; }
            set { this.RaiseAndSetIfChanged(ref _Description, value); }
        }
    }
}
