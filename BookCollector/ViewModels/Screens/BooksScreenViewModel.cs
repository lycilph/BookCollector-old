﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using BookCollector.Domain;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Services.Search;
using BookCollector.ViewModels.Common;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace BookCollector.ViewModels.Screens
{
    public class BooksScreenViewModel : ScreenBase, IHandle<ApplicationMessage>
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IApplicationModel application_model;
        private IEventAggregator event_aggregator;
        private ISnackbarMessageQueue message_queue;
        private ISearchEngine search_engine;
        private List<SearchResult> search_results;

        private ReactiveList<ShelfViewModel> _Shelves;
        public ReactiveList<ShelfViewModel> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

        private ShelfViewModel _SelectedShelf;
        public ShelfViewModel SelectedShelf
        {
            get { return _SelectedShelf; }
            set { this.RaiseAndSetIfChanged(ref _SelectedShelf, value); }
        }

        private ICollectionView _Books;
        public ICollectionView Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private ReactiveList<string> _SortProperties;
        public ReactiveList<string> SortProperties
        {
            get { return _SortProperties; }
            set { this.RaiseAndSetIfChanged(ref _SortProperties, value); }
        }

        private string _SelectedSortProperty;
        public string SelectedSortProperty
        {
            get { return _SelectedSortProperty; }
            set { this.RaiseAndSetIfChanged(ref _SelectedSortProperty, value); }
        }

        private bool _SortAscending;
        public bool SortAscending
        {
            get { return _SortAscending; }
            set { this.RaiseAndSetIfChanged(ref _SortAscending, value); }
        }

        public BooksScreenViewModel(IApplicationModel application_model, IEventAggregator event_aggregator, ISnackbarMessageQueue message_queue, ISearchEngine search_engine)
        {
            this.application_model = application_model;
            this.event_aggregator = event_aggregator;
            this.message_queue = message_queue;
            this.search_engine = search_engine;

            DisplayName = Constants.BooksScreenDisplayName;
            event_aggregator.Subscribe(this);

            SortProperties = new ReactiveList<string> { "Title", "Authors" };
            SelectedSortProperty = SortProperties.First();
            SortAscending = true;

            this.WhenAnyValue(x => x.SelectedSortProperty, x => x.SortAscending)
                .Subscribe(_ => UpdateSorting());

            this.WhenAnyValue(x => x.SelectedShelf)
                .Subscribe(_ => 
                {
                    Books?.Refresh();
                    Books?.MoveCurrentToFirst();
                });
        }

        public override void Activate()
        {
            base.Activate();

            var vms = application_model.CurrentCollection.Books.Select(b => new BookViewModel(b));
            Books = CollectionViewSource.GetDefaultView(vms);
            Books.Filter = Filter;

            UpdateSorting();

            Shelves = application_model.CurrentCollection.Shelves.Select(s => new ShelfViewModel(s)).ToReactiveList();
            Shelves.Apply(s => s.BooksCount = application_model.CurrentCollection.BooksOnShelf(s.Unwrap()));
            SelectedShelf = Shelves.FirstOrDefault();

            if (!application_model.CurrentCollection.Books.Any())
                message_queue.Enqueue("No Books?", "Import Here", () => event_aggregator.Publish(ApplicationMessage.NavigateTo(Constants.ImportScreenDisplayName)));
        }

        private void UpdateSorting()
        {
            if (Books == null)
                return;

            var direction = (SortAscending ? ListSortDirection.Ascending : ListSortDirection.Descending);

            var primary_sort_property = SelectedSortProperty;
            var secondary_sort_property = SortProperties.First(p => p != primary_sort_property);

            Books.SortDescriptions.Clear();
            Books.SortDescriptions.Add(new SortDescription(primary_sort_property, direction));
            Books.SortDescriptions.Add(new SortDescription(secondary_sort_property, direction));
        }

        private bool Filter(object o)
        {
            var book = o as BookViewModel;
            if (book == null || SelectedShelf == null)
                return false;

            if (search_results != null)
                return search_results.Any(s => s.Book.Title == book.Title) && book.Shelves.Any(s => s.Name == SelectedShelf.Name);
            else
                return book.Shelves.Any(s => s.Name == SelectedShelf.Name);
        }

        public void Handle(ApplicationMessage message)
        {
            if (message.Kind == ApplicationMessage.MessageKind.SearchTextChanged)
            {
                log.Info($"Search text changed - {message.Text}");
                var sw = Stopwatch.StartNew();
                search_results = search_engine.Search(message.Text);
                sw.Stop();
                log.Info($"Search took {sw.ElapsedMilliseconds} ms and gave {(search_results == null ? 0 : search_results.Count)} results");

                Books?.Refresh();
                Books?.MoveCurrentToFirst();
            }
        }
    }
}
