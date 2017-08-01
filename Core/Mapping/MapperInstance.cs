using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.Mapping
{
    public class MapperInstance
    {
        private Dictionary<Tuple<Type, Type>, object> maps = new Dictionary<Tuple<Type, Type>, object>();

        public void Add<TSource, TDestination>(Action<TSource, TDestination> action)
        {
            var key = Tuple.Create(typeof(TSource), typeof(TDestination));
            maps.Add(key, action);
        }

        public TDestination Map<TDestination>(object source) where TDestination : new()
        {
            var destination = new TDestination();
            Map(source, destination);
            return destination;
        }

        public void Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            var source_properties = source.GetType()
                                          .GetProperties(BindingFlags.FlattenHierarchy |
                                                         BindingFlags.Public |
                                                         BindingFlags.Instance);

            var source_type = source.GetType(); // Needed as source might be of type "object", when called via Map<TDestination>(object source)
            var destination_type = destination.GetType();

            // Copy values for properties with same type and name
            foreach (var sp in source_properties)
            {
                if (sp.CanRead && sp.GetGetMethod() != null)
                {
                    var tp = destination_type.GetProperty(sp.Name);

                    if (tp != null && sp.PropertyType == tp.PropertyType && tp.CanWrite && tp.GetSetMethod() != null)
                        tp.SetValue(destination, sp.GetValue(source));
                }
            }

            // Check and execute custom rules
            var key = Tuple.Create(source_type, destination_type);
            if (maps.TryGetValue(key, out object action))
            {
                var parameters = new object[] { source, destination };
                action.GetType().GetMethod("Invoke").Invoke(action, parameters);
            }
        }
    }
}
