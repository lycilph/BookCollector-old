using System;

namespace BookCollector.Framework.Mapping
{
    // Inspired by the following sources:
    // - https://weblogs.asp.net/psteele/super-simple-object-mapper
    // - https://github.com/omuleanu/ValueInjecter
    public class Mapper
    {
        public static readonly MapperInstance Instance = new MapperInstance();

        public static void Add<TSource, TDestination>(Action<TSource, TDestination> action)
        {
            Instance.Add<TSource, TDestination>(action);
        }

        public static TDestination Map<TDestination>(object source) where TDestination : new()
        {
            return Instance.Map<TDestination>(source);
        }

        public static void Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            Instance.Map<TSource, TDestination>(source, destination);
        }
    }
}
