using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using BookCollector.Api.SearchProvider;
using BookCollector.Data;
using Panda.ApplicationCore.Utilities;
using ReactiveUI;

namespace BookCollector.Screens.Search
{
    public class SearchProviderViewModel : ItemViewModelBase<ISearchProvider>
    {
        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsBusy, value); }
        }

        public string Image { get; private set; }

        public SearchProviderViewModel(ISearchProvider obj) : base(obj)
        {
            var assembly_name = Assembly.GetExecutingAssembly().GetName().Name;
            Image = string.Format("/{0};component/{1}", assembly_name, obj.Image);
        }

        public async Task<List<Book>> Search(string text)
        {
            IsBusy = true;
            var books = await AssociatedObject.Search(text);
            IsBusy = false;
            return books;
        }
    }
}
