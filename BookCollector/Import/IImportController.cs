using System.Collections.Generic;

namespace BookCollector.Import
{
    public interface IImportController
    {
        string Name { get; }
        
        IEnumerable<ImportStepViewModel> Steps { get; }
        
        void Start();
    }
}
