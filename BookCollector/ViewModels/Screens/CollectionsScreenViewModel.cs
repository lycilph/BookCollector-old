using BookCollector.Domain;
using BookCollector.Framework.MVVM;

namespace BookCollector.ViewModels.Screens
{
    public class CollectionsScreenViewModel : ScreenBase
    {
        public CollectionsScreenViewModel()
        {
            DisplayName = Constants.CollectionsScreenDisplayName;
        }
    }
}
