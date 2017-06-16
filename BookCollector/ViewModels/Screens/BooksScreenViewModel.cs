using BookCollector.Domain;
using BookCollector.Framework.MVVM;

namespace BookCollector.ViewModels.Screens
{
    public class BooksScreenViewModel : ScreenBase
    {
        public BooksScreenViewModel()
        {
            DisplayName = Constants.BooksScreenDisplayName;
        }
    }
}
