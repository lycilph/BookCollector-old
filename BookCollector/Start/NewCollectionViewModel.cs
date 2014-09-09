using System.IO;
using System;
using System.Reactive.Linq;
using BookCollector.Data;
using Framework.Dialogs;
using Framework.Mvvm;
using ReactiveUI;

namespace BookCollector.Start
{
    public class NewCollectionViewModel : ItemViewModelBase<Info>, ICanOk
    {
        private readonly string dir;
        private bool update_name = true;
        private bool updating_filename;

        public string DisplayName
        {
            get { return AssociatedObject.DisplayName; }
            set { AssociatedObject.DisplayName = value; }
        }

        public string Filename
        {
            get { return Path.GetFileName(AssociatedObject.Filename); }
            set { AssociatedObject.Filename = Path.Combine(dir, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _CanOk;
        public bool CanOk
        {
            get { return _CanOk.Value; }
        }

        public NewCollectionViewModel(Info info) : base(info)
        {
            dir = Path.GetDirectoryName(info.Filename);

            this.WhenAnyValue(x => x.Filename)
                .Subscribe(s =>
                {
                    if (!update_name) return;

                    updating_filename = true;
                    DisplayName = Path.GetFileNameWithoutExtension(s);
                    updating_filename = false;
                });

            this.WhenAnyValue(x => x.DisplayName)
                .Subscribe(s =>
                {
                    if (updating_filename) return;

                    update_name = (DisplayName == Path.GetFileNameWithoutExtension(Filename));
                });

            _CanOk = this.WhenAnyValue(x => x.AssociatedObject.Filename)
                         .Select(x =>!File.Exists(x))
                         .ToProperty(this, x => x.CanOk);
        }
    }
}
