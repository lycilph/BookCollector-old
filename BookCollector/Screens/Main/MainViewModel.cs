using System.ComponentModel.Composition;
using Caliburn.Micro.ReactiveUI;

namespace BookCollector.Screens.Main
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : ReactiveScreen
    {
    }
}
