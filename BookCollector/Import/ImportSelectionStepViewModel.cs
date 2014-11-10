﻿using BookCollector.Services.Goodreads;
using BookCollector.Services.GoogleBooks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;

namespace BookCollector.Import
{
    public sealed class ImportSelectionStepViewModel : ReactiveScreen
    {
        private readonly ImportViewModel parent;

        public ImportSelectionStepViewModel(ImportViewModel parent)
        {
            this.parent = parent;
            DisplayName = "Selection";
        }

        public void SelectGoodreads()
        {
            var api = IoC.Get<GoodreadsApi>();
            parent.Import(api);
        }

        public void SelectGoogleBooks()
        {
            var api = IoC.Get<GoogleBooksApi>();
            parent.Import(api);
        }
    }
}
