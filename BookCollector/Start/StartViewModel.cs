using System.ComponentModel.Composition;
using System;
using System.IO;
using System.Reactive.Linq;
using BookCollector.Data;
using BookCollector.Manager;
using BookCollector.Shell;
using Caliburn.Micro;
using Framework.Dialogs;
using Framework.Docking;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using ReactiveUI;

namespace BookCollector.Start
{
    [Export(typeof(IStartTool))]
    public class StartViewModel : ToolScreen, IStartTool
    {
        private readonly IEventAggregator event_aggregator;
        private readonly ApplicationSettings application_settings;
        private readonly ContentManager content_manager;
        private readonly InfoRepository repository;

        public override bool StartDockedAsDocument { get { return true; } }

        private InfoViewModel _CurrentItem;
        public InfoViewModel CurrentItem
        {
            get { return _CurrentItem; }
            set { this.RaiseAndSetIfChanged(ref _CurrentItem, value); }
        }

        private IReactiveDerivedList<InfoViewModel> _Items;
        public IReactiveDerivedList<InfoViewModel> Items
        {
            get { return _Items; }
            set { this.RaiseAndSetIfChanged(ref _Items, value); }
        }

        [ImportingConstructor]
        public StartViewModel(InfoRepository info_repository, IEventAggregator event_aggregator, ContentManager content_manager, ApplicationSettings application_settings)
        {
            repository = info_repository;
            this.event_aggregator = event_aggregator;
            this.content_manager = content_manager;
            this.application_settings = application_settings;
            Items = repository.Items.CreateDerivedCollection(i => new InfoViewModel(i));

            //this.WhenAnyValue(x => x.CurrentItem)
            //    .Where(i => i != null)
            //    .Subscribe(i => ShowCollection(i.AssociatedObject));
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            DisplayName = "Start";
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            CurrentItem = null;
        }

        private async void ShowCollection(Info info)
        {
            if (!info.HasValidCollection())
            {
                var settings = new MetroDialogSettings {AffirmativeButtonText = "Yes", NegativeButtonText = "No"};
                var result = await DialogController.ShowMessage("Collection now found", "Do you want to remove it?", MessageDialogStyle.AffirmativeAndNegative, settings);
                if (result == MessageDialogResult.Affirmative)
                    repository.Remove(info);
                return;
            }

            var main = content_manager.GetOrCreate(info);
            event_aggregator.PublishOnCurrentThread(ShellMessage.Show(main));

            if (!application_settings.KeepStartOpen)
                OnClose();
        }

        public void ShowCollection()
        {
            ShowCollection(CurrentItem.AssociatedObject);
        }

        public async void NewCollection()
        {
            var info = repository.Create();
            var vm = new NewCollectionViewModel(info);
            var result = await DialogController.ShowAsync(vm);
            if (result == MessageDialogResult.Affirmative)
            {
                info.CreateCollection();
                repository.Add(info);
                ShowCollection(info);
            }
        }

        public void OpenCollection()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "text files (.txt)|*.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                var name = Path.GetFileNameWithoutExtension(dialog.FileName);
                var info = repository.Create(name, dialog.FileName);
                repository.Add(info);
                ShowCollection(info);
            }
        }
    }
}
