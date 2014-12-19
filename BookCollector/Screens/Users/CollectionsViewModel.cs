using System.ComponentModel.Composition;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;

namespace BookCollector.Screens.Users
{
    [Export("Collections", typeof(IScreen))]
    public class CollectionsViewModel : ReactiveScreen
    {
    }
}
