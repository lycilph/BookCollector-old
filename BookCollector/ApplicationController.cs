using System;
using System.ComponentModel.Composition;
using BookCollector.Model;
using BookCollector.Screens;
using BookCollector.Services.Settings;
using BookCollector.Shell;
using Caliburn.Micro;

namespace BookCollector
{
    [Export(typeof(ApplicationController))]
    public class ApplicationController : IHandle<ApplicationMessage>
    {
        private readonly IEventAggregator event_aggregator;
        private readonly ProfileController profile_controller;

        [ImportingConstructor]
        public ApplicationController(IEventAggregator event_aggregator, ProfileController profile_controller)
        {
            this.event_aggregator = event_aggregator;
            this.profile_controller = profile_controller;

            event_aggregator.Subscribe(this);
        }

        public void Activate()
        {
            ApplicationSettings.Instance.Load();
            profile_controller.Load();
        }

        public void Deactivate()
        {
            ApplicationSettings.Instance.Save();
            profile_controller.Save();
        }

        public void Handle(ApplicationMessage message)
        {
            switch (message.Kind)
            {
                case ApplicationMessage.MessageKind.ShellInitialized:
                    Initialize();
                    break;
                case ApplicationMessage.MessageKind.ShowProfiles:
                    ShowProfiles();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ShowProfiles()
        {
            var profiles = IoC.Get<IShellScreen>("Profiles");
            event_aggregator.PublishOnUIThread(ShellMessage.Show(profiles));
        }

        private void Initialize()
        {
            var main = IoC.Get<IShellScreen>("Main");
            event_aggregator.PublishOnUIThread(ShellMessage.Show(main));

            if (ApplicationSettings.Instance.RememberLastCollection == false || profile_controller.CurrentCollection == null)
            {
                var profiles = IoC.Get<IShellScreen>("Profiles");
                event_aggregator.PublishOnUIThread(ShellMessage.Show(profiles));
            }
        }
    }
}
