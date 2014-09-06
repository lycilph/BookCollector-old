using System;
using System.Linq;
using System.Reflection;

namespace BookCollector.Utils
{
    // See: http://stackoverflow.com/questions/286294/object-to-object-mapper
    public static class Mapper
    {
        public static T2 MapPublicProperties<T1, T2>(T1 source, T2 target) where T1 : class where T2 : class
        {
            if (source == null || target == null)
                throw new ArgumentException();

            var source_properties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var target_properties = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var properties = source_properties.Join(target_properties, p => p.Name, p => p.Name, (p1, p2) => new {p1, p2})
                                              .Where(a => a.p2.CanWrite);
            foreach (var property_pair in properties)
            {
                //They have to have the same return type
                if (property_pair.p1.PropertyType.Name != property_pair.p2.PropertyType.Name)
                    continue;

                //Get the value from the mapFrom. Caveat: Indexing properties not supported!
                var from_value = property_pair.p1.GetValue(source, null);
                property_pair.p2.SetValue(target, from_value, null);
            }

            return target;
        }
    }
}
