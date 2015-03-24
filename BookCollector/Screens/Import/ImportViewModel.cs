using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookCollector.Api.ImportProvider;
using BookCollector.Controllers;
using BookCollector.Data;
using BookCollector.Screens.Import.Dialog;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Panda.ApplicationCore.Dialogs;
using System.ComponentModel.Composition;
using Panda.Utilities.Extensions;
using ReactiveUI;
using EnumerableExtensions = Caliburn.Micro.EnumerableExtensions;

namespace BookCollector.Screens.Import
{
    [Export(typeof(ImportViewModel))]
    public class ImportViewModel : BookCollectorScreenBase
    {
        private const int StatusPage = 0;
        private const int ResultsPage = 1;

        private readonly INavigationController navigation_controller;
        private readonly IStatusController status_controller;
        private readonly IDataController data_controller;
        private readonly SettingsViewModel settings_flyout = new SettingsViewModel();

        private int _Page;
        public int Page
        {
            get { return _Page; }
            set { this.RaiseAndSetIfChanged(ref _Page, value); }
        }

        private ReactiveList<string> _Messages = new ReactiveList<string>();
        public ReactiveList<string> Messages
        {
            get { return _Messages; }
            set { this.RaiseAndSetIfChanged(ref _Messages, value); }
        }

        private ReactiveList<ImportBookViewModel> _Books = new ReactiveList<ImportBookViewModel>();
        public ReactiveList<ImportBookViewModel> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private ReactiveList<ColumnViewModel> _Columns;
        public ReactiveList<ColumnViewModel> Columns
        {
            get { return _Columns; }
            set { this.RaiseAndSetIfChanged(ref _Columns, value); }
        }

        private bool? _IsAllSelected;
        public bool? IsAllSelected
        {
            get { return _IsAllSelected; }
            set { this.RaiseAndSetIfChanged(ref _IsAllSelected, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _CanBack;
        public bool CanBack { get { return _CanBack.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanOk;
        public bool CanOk { get { return _CanOk.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanCancel;
        public bool CanCancel { get { return _CanCancel.Value; } }
            
        [ImportingConstructor]
        public ImportViewModel(INavigationController navigation_controller, IStatusController status_controller, IDataController data_controller)
        {
            this.navigation_controller = navigation_controller;
            this.status_controller = status_controller;
            this.data_controller = data_controller;

            Columns = new ReactiveList<ColumnViewModel>
            {
                new ColumnViewModel { PropertyName = "Selected", UseTemplate = true, CanSetVisibility = false },
                new ColumnViewModel { Name = "Duplicate", UseTemplate = true, CanSetVisibility = false },
                new ColumnViewModel { Name = "Title" },
                new ColumnViewModel { Name = "Author(s)", PropertyName = "Authors" },
                new ColumnViewModel { Name = "ISBN", PropertyName = "ISBN10", IsVisible = false },
                new ColumnViewModel { Name = "ISBN13", PropertyName = "ISBN13" },
                new ColumnViewModel { Name = "Shelf" }
            };

            Books.ChangeTrackingEnabled = true;
            var books_changed = Observable.Merge(Books.ItemChanged.Select(_ => Unit.Default), 
                                                 Books.CountChanged.Select(_ => Unit.Default));
         
            books_changed.Subscribe(_ =>
            {
                var selected_count = Books.Count(b => b.IsSelected);
                IsAllSelected = selected_count == 0 ? false :
                                selected_count == Books.Count ? (bool?)(true) :
                                null;
            });

            _CanBack = status_controller.WhenAny(x => x.IsBusy, x => !x.Value)
                                        .ToProperty(this, x => x.CanBack);

            _CanOk = Observable.Zip(books_changed.Select(_ => Books.Any(b => b.IsSelected)), 
                                    status_controller.WhenAnyValue(x => x.IsBusy),
                                    (x, y) => x && !y)
                               .ToProperty(this, x => x.CanOk);

            _CanCancel = status_controller.WhenAny(x => x.IsBusy, x => !x.Value)
                                          .ToProperty(this, x => x.CanCancel);
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            Reset();
            status_controller.AddFlyout(settings_flyout);

            var selection = IoC.Get<ImportProviderSelectionViewModel>();
            var result = await DialogController.ShowViewModel(selection);
            if (result == MessageDialogResult.Affirmative)
                await Import(selection.SelectedImportProvider);
            else
                Back();

        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            status_controller.RemoveFlyout(settings_flyout);
        }

        private void Reset()
        {
            Messages.Clear();
            Books.Clear();
            Page = StatusPage;
            IsAllSelected = false;
        }

        private async Task Import(IImportProvider import_provider)
        {
            var status = new Progress<string>(str => Messages.Add(str));
            using (var disp = status_controller.BusyWithMessage("Importing from " + import_provider.Name))
            {
                var imported_books = await import_provider.Execute(status);

                await Task.Delay(500);
                Page = ResultsPage;

                if (imported_books.Any())
                {
                    var total = 0;
                    await imported_books
                        .ToObservable()
                        .Regulate(TimeSpan.FromMilliseconds(20))
                        .ObserveOnDispatcher()
                        .Do(b =>
                        {
                            total++;
                            status_controller.AuxiliaryStatusText = "Books: " + total;

                            var book = Mapper.Map<Book>(b);
                            var is_duplicate = data_controller.IsDuplicate(book);
                            Books.Add(new ImportBookViewModel(b, is_duplicate));
                        });
                }
                else
                {
                    status_controller.AuxiliaryStatusText = "Nothing found";
                }
            }
            status_controller.MainStatusText = "Done importing";
        }

        public void Back()
        {
            navigation_controller.Back();
        }

        public void UpdateSelection()
        {
            EnumerableExtensions.Apply(Books, b => b.IsSelected = IsAllSelected == true);
        }

        public void SelectNonDuplicates()
        {
            EnumerableExtensions.Apply(Books, b => b.IsSelected = !b.IsDuplicate);
        }

        public void ShowSettings()
        {
            settings_flyout.IsOpen = true;
        }

        public void Ok()
        {
            if (settings_flyout.ImportShelves)
            {
                var imports = Books.Where(b => b.IsSelected)
                                   .Select(b => b.AssociatedObject)
                                   .GroupBy(b => b.Shelf);
                foreach (var import in imports)
                {
                    var shelf = data_controller.Collection.GetOrCreate(import.Key);
                    var books = Mapper.Map<List<Book>>(import);
                    EnumerableExtensions.Apply(books, b => b.History.Add("Imported on " + DateTime.Now.ToShortDateString()));
                    shelf.AddRange(books);
                    data_controller.Collection.All.AddRange(books);
                }
            }
            else
            {
                var imported_books = Books.Where(b => b.IsSelected).Select(b => b.AssociatedObject);
                var books = Mapper.Map<List<Book>>(imported_books);
                EnumerableExtensions.Apply(books, b => b.History.Add("Imported on " + DateTime.Now.ToShortDateString()));
                data_controller.Collection.All.AddRange(books);
            }

            navigation_controller.Back();            
        }

        public void Cancel()
        {
            navigation_controller.Back();            
        }
    }
}
