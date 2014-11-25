using System;
using ReactiveUI;
using IScreen = Caliburn.Micro.IScreen;

namespace BookCollector.Old
{
    public class ImportStepViewModel : ReactiveObject
    {
        public IScreen Screen { get; private set; }

        public string DisplayName { get { return Screen.DisplayName; } }

        private bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }
            set { this.RaiseAndSetIfChanged(ref _IsActive, value); }
        }

        public ImportStepViewModel(IScreen screen)
        {
            Screen = screen;
            screen.WhenAnyValue(x => x.IsActive)
                  .Subscribe(x => IsActive = x);
        }
    }
}
