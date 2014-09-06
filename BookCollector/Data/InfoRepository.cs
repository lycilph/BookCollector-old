using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using DesktopOrganizer.Utils;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    [Export(typeof(InfoRepository))]
    public class InfoRepository : ReactiveObject, IEnumerable<Info>
    {
        private readonly ApplicationSettings application_settings;
        private const string RepositoryFilename = "Info.txt";
        private const string DefaultName = "Default";
        private const string DefaultFilename = "Default.txt";

        public ReactiveList<Info> Items { get; private set; }

        [ImportingConstructor]
        public InfoRepository(ApplicationSettings application_settings)
        {
            this.application_settings = application_settings;
            Items = new ReactiveList<Info>();
        }

        public IEnumerator<Info> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Info Create()
        {
            var info = new Info
            {
                DisplayName = DefaultName,
                Filename = application_settings.GetFilename(DefaultFilename)
            };
            return info;
        }

        public void Add(Info info)
        {
            Items.Add(info);
        }

        public void Remove(Info info)
        {
            Items.Remove(info);
        }

        public void Load()
        {
            var path = application_settings.GetFilename(RepositoryFilename);
            if (!File.Exists(path))
                return;

            var json = File.ReadAllText(path);
            Items = JsonConvert.DeserializeObject<List<Info>>(json).ToReactiveList();
        }

        public void Save()
        {
            var path = application_settings.GetFilename(RepositoryFilename);
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            var json = JsonConvert.SerializeObject(Items, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
