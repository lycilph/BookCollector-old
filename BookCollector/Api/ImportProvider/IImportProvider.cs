﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookCollector.Api.ImportProvider
{
    public interface IImportProvider
    {
        string Name { get; }

        Task Execute(IProgress<string> status, IProgress<List<ImportedBook>> results);
    }
}
