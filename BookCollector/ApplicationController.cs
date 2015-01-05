using System;
using System.ComponentModel.Composition;
using BookCollector.Model;
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
        }

        public void Deactivate()
        {
            ApplicationSettings.Instance.Save();
        }

        public void Handle(ApplicationMessage message)
        {
            switch (message.Kind)
            {
                case ApplicationMessage.MessageKind.ShellInitialized:
                    Initialize();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Initialize()
        {
            var main = IoC.Get<IScreen>("Main");
            event_aggregator.PublishOnUIThread(ShellMessage.Show(main));

            if (!ApplicationSettings.Instance.RememberLastCollection)
            {
                var profiles = IoC.Get<IScreen>("Profiles");
                event_aggregator.PublishOnUIThread(ShellMessage.Show(profiles));
            }
        }
    }
}
