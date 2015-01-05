﻿using Caliburn.Micro;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Model
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ProfileDescription : ReactiveObject, IHaveDisplayName
    {
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        private ReactiveList<CollectionDescription> _Collections = new ReactiveList<CollectionDescription>();
        public ReactiveList<CollectionDescription> Collections
        {
            get { return _Collections; }
            set { this.RaiseAndSetIfChanged(ref _Collections, value); }
        }

    }
}
