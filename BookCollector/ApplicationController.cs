using System;
using System.ComponentModel.Composition;
using BookCollector.Model;
using BookCollector.Screens;
using BookCollector.Services;
using BookCollector.Shell;
using Caliburn.Micro;

namespace BookCollector
{
    [Export(typeof(ApplicationController))]
    public class ApplicationController : IHandle<ApplicationMessage>
    {
        private readonly IEventAggregator event_aggregator;
        private readonly ApplicationSettings application_settings;
        private readonly ProfileController profile_controller;

        [ImportingConstructor]
        public ApplicationController(IEventAggregator event_aggregator, ProfileController profile_controller, ApplicationSettings application_settings)
        {
            this.event_aggregator = event_aggregator;
            this.profile_controller = profile_controller;
            this.application_settings = application_settings;

            event_aggregator.Subscribe(this);
        }

        public void Activate()
        {
            application_settings.Load();
            profile_controller.Load(application_settings.DataDir);
        }

        public void Deactivate()
        {
            application_settings.Save();
            profile_controller.Save(application_settings.DataDir);
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

            if (application_settings.RememberLastCollection == false || profile_controller.CurrentCollection == null)
            {
                var profiles = IoC.Get<IShellScreen>("Profiles");
                event_aggregator.PublishOnUIThread(ShellMessage.Show(profiles));
            }
        }
    }
}
