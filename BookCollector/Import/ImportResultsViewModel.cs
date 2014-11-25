using System.ComponentModel.Composition;
using Caliburn.Micro.ReactiveUI;

namespace BookCollector.Import
{
    [Export(typeof(ImportResultsViewModel))]
    public class ImportResultsViewModel : ReactiveScreen
    {
    }
}
