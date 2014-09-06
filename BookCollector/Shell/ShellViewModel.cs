using BookCollector.Main;
using BookCollector.Manager;
using Caliburn.Micro;
using Framework.Core;
using Framework.Docking;
using Framework.MainMenu.ViewModels;
using Framework.Module;
using Framework.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace BookCollector.Shell
{
    [Export(typeof (IShell))]
    public class ShellViewModel : DockingShell, IHandle<ShellMessage>
    {
        private readonly ContentManager content_manager;
        private readonly List<IModule> modules;

        [ImportingConstructor]
        public ShellViewModel([ImportMany] IEnumerable<Lazy<IModule, IOrderMetadata>> modules, IMenu menu, IEventAggregator event_aggregator, ContentManager content_manager)
        {
            this.content_manager = content_manager;
            this.modules = modules.OrderBy(obj => obj.Metadata.Order).Select(obj => obj.Value).ToList();

            Menu = menu;
            Menu.Add(new MenuItem("_File")
            {
                new MenuItem("E_xit", () => TryClose())
            });

            event_aggregator.Subscribe(this);
            this.modules.Apply(m => m.Create(this));
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            DisplayName = "Book Collector";
            modules.Apply(m => m.Initialize());
        }

        public override void ActivateItem(ILayoutItem item)
        {
            if (item is ITool)
            {
                var tool = item as ITool;
                tool.IsVisible = true;
                tool.IsSelected = true;
            }

            base.ActivateItem(item);
        }

        public override void DeactivateItem(ILayoutItem item, bool close)
        {
            base.DeactivateItem(item, close);

            if (item is IContent)
                content_manager.Remove(item as IContent);
        }

        public void Handle(ShellMessage message)
        {
            switch (message.Kind)
            {
                case ShellMessage.MessageKind.Show:
                {
                    ActivateItem(message.Item);
                }
                break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
