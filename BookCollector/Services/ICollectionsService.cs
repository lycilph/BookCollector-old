﻿using System.Collections.Generic;
using BookCollector.Data;

namespace BookCollector.Services
{
    public interface ICollectionsService
    {
        Collection Current { get; set; }

        void Initialize();
        void Exit();
        IEnumerable<Collection> GetAllCollections();
    }
}