using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Model;
using BookCollector.Services;
using BookCollector.Shell;
using Caliburn.Micro;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Controllers
{
    [Export(typeof(ApplicationController))]
    public class ApplicationController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator event_aggregator;
        private readonly ApplicationSettings application_settings;
        private readonly ProfileController profile_controller;
        private readonly BookRepository book_repository;
        private readonly ImageDownloader image_downloader;

        public ProfileController ProfileController { get { return profile_controller; } }

        public BookRepository BookRepository { get { return book_repository; } }

        [ImportingConstructor]
        public ApplicationController(IEventAggregator event_aggregator,
                                     ProfileController profile_controller, 
                                     ApplicationSettings application_settings, 
                                     BookRepository book_repository, 
                                     ImageDownloader image_downloader)
        {
            this.event_aggregator = event_aggregator;
            this.profile_controller = profile_controller;
            this.application_settings = application_settings;
            this.book_repository = book_repository;
            this.image_downloader = image_downloader;

            event_aggregator.Subscribe(this);
        }

        public void Activate()
        {
            logger.Trace("Activate");

            application_settings.Load();
            profile_controller.Load();
        }

        public void Deactivate()
        {
            logger.Trace("Deactivate");

            SaveCollection();

            application_settings.Save();
            profile_controller.Save();
        }

        public void NavigateBack()
        {
            logger.Trace("NavigateBack");
            event_aggregator.PublishOnUIThread(ShellMessage.Back());
        }

        public void NavigateToProfiles()
        {
            logger.Trace("NavigateToProfiles");
            event_aggregator.PublishOnUIThread(ShellMessage.Show("Profiles"));
        }

        public void NavigateToImport()
        {
            logger.Trace("NavigateToImport");
            event_aggregator.PublishOnUIThread(ShellMessage.Show("Import"));
        }

        public void NavigateToSettings()
        {
            logger.Trace("NavigateToSettings");
            event_aggregator.PublishOnUIThread(ShellMessage.Show("Settings"));
        }

        public void SetStatusText(string message)
        {
            event_aggregator.PublishOnUIThread(ShellMessage.StatusText(message));
        }

        public void SetBusy(bool state)
        {
            event_aggregator.PublishOnUIThread(ShellMessage.Busy(state));
        }

        public void Initialize()
        {
            logger.Trace("Initialize");

            UpdateCommandText();
            LoadCollection();

            event_aggregator.PublishOnUIThread(ShellMessage.Show("Main"));

            if (application_settings.RememberLastCollection == false || profile_controller.CurrentCollection == null)
                event_aggregator.PublishOnUIThread(ShellMessage.Show("Profiles"));
        }

        public void Import(List<ImportedBook> imported_books)
        {
            book_repository.Add(imported_books.Select(i => i.Book));
            image_downloader.Add(imported_books.Select(i => new DownloadQueueItem(i.Book, i.ImageLinks)));
            profile_controller.CurrentCollection.LastModified = DateTime.Now;
        }

        public void Clear()
        {
            book_repository.Clear();
            image_downloader.Clear();
            profile_controller.CurrentCollection.LastModified = DateTime.Now;
        }

        public void SetCurrent(CollectionDescription collection)
        {
            logger.Trace("SetCurrent({0})", collection.DisplayName);

            if (profile_controller.CurrentCollection == collection)
                return;

            if (profile_controller.CurrentCollection != null)
                SaveCollection();

            profile_controller.SetCurrent(collection);
            UpdateCommandText();

            if (profile_controller.CurrentCollection != null)
                LoadCollection();
        }

        private void UpdateCommandText()
        {
            var profile_name = (profile_controller.CurrentProfile == null ? "" : profile_controller.CurrentProfile.DisplayName);
            var collection_name = (profile_controller.CurrentCollection == null ? "" : profile_controller.CurrentCollection.DisplayName);
            var command_text = string.Format("{0} [{1}]", profile_name, collection_name);
            event_aggregator.PublishOnUIThread(ShellMessage.CommandText(command_text));
        }

        private void SaveCollection()
        {
            logger.Trace("SaveCollection");
            book_repository.Save(profile_controller.CurrentCollection);
            image_downloader.Stop();
            image_downloader.Save(profile_controller.CurrentCollection);
            // Close search index ???
        }

        private void LoadCollection()
        {
            logger.Trace("LoadCollection");

            book_repository.Load(profile_controller.CurrentCollection);
            image_downloader.Load(profile_controller.CurrentCollection, book_repository.Get);
            image_downloader.Start();
            // Load search index
        }
    }
}
