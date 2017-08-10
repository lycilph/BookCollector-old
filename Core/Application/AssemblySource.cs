using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Application
{
    public static class AssemblySource
    {
        public static readonly List<Assembly> Assemblies = new List<Assembly>();

        public static Type GetType(string name)
        {
            var all_types = Assemblies.SelectMany(a => a.ExportedTypes);
            var type = all_types.FirstOrDefault(t => t.FullName == name);

            return type;
        }
    }
}
