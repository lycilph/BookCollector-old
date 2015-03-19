using System.Collections.Generic;
using BookCollector.Data;

namespace BookCollector.Controllers
{
    public interface IDataController
    {
        User User { get; set; }
        Collection Collection { get; set; }
        void Load();
        void Save();
        void Save(List<User> users);
        List<User> GetAllUsers();
        void Delete(User user);
        bool IsDuplicate(Book book);
    }
}