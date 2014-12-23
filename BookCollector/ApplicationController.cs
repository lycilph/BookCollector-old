using System.Collections.Generic;
using System.ComponentModel.Composition;
using BookCollector.Services.Books;
using BookCollector.Services.Collections;
using BookCollector.Services.Settings;
using BookCollector.Utilities;
using Caliburn.Micro;

namespace BookCollector
{
    [Export(typeof(ApplicationController))]
    public class ApplicationController
    {
        private readonly List<IPersistable> persistables;

        [ImportingConstructor]
        public ApplicationController(ApplicationSettings settings, CollectionsController collections_controller, CollectionController collection_controller)
        {
            persistables = new List<IPersistable>
            {
                settings,
                collections_controller,
                collection_controller
            };
        }

        public void Activate()
        {
            persistables.Apply(p => p.Load());
        }

        public void Deactivate()
        {
            persistables.Apply(p => p.Save());
        }
    }
}
