using System.Collections.Generic;
using BookCollector.Data;

namespace BookCollector.Models
{
    public interface IBookCollectorModel
    {
        Collection CurrentCollection { get; set; }

        List<CollectionDescription> GetAllCollectionDescriptions();

        //void LoadData();
        //void SaveData();
    }
}