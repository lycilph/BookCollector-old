using System.Collections.Generic;
using System.ComponentModel.Composition;
using BookCollector.Data;
using BookCollector.Main;
using Caliburn.Micro;
using Framework.Docking;

namespace BookCollector.Manager
{
    [Export(typeof(ContentManager))]
    public class ContentManager
    {
        private readonly Dictionary<Info, IMain> content_dictionary = new Dictionary<Info, IMain>();

        public IMain GetOrCreate(Info info)
        {
            if (content_dictionary.ContainsKey(info))
                return content_dictionary[info];

            var main = IoC.Get<IMain>();
            main.Info = info;
            content_dictionary.Add(info, main);
            return main;
        }

        public void Remove(IContent content)
        {
            if (content is IMain)
            {
                var main = content as IMain;
                content_dictionary.Remove(main.Info);                
            }
        }
    }
}
