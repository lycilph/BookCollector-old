﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Panda.ApplicationCore.Utilities;
using ReactiveUI;

namespace BookCollector.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class User : DirtyTrackingReactiveObject
    {
        private string _Id = Guid.NewGuid().ToString().ToUpperInvariant();
        public string Id
        {
            get { return _Id; }
            set { this.RaiseAndSetIfChanged(ref _Id, value); } // This is used when deserializing
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private ReactiveList<Collection> _Collections = new ReactiveList<Collection>();
        public ReactiveList<Collection> Collections
        {
            get { return _Collections; }
            set { this.RaiseAndSetIfChanged(ref _Collections, value); }
        }

        private Dictionary<string, string> _Credentials = new Dictionary<string, string>();
        public Dictionary<string, string> Credentials
        {
            get { return _Credentials; }
            set { this.RaiseAndSetIfChanged(ref _Credentials, value); }
        }

        public User() : this("[User]") { }
        public User(string name)
        {
            Name = name;
            IsDirty = false;
        }

        public void Add(Collection collection)
        {
            Collections.Add(collection);
        }

        public void Remove(Collection collection)
        {
            Collections.Remove(collection);
        }
    }
}