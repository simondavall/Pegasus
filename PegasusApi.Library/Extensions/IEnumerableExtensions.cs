using System.Collections.Generic;
using System.Linq;

namespace PegasusApi.Library.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> AddRange<T>(this IEnumerable<T> items, IEnumerable<T> additionalItems)
        {
            return items == null ? additionalItems : items.Concat(additionalItems);
        }

        public static IEnumerable<T> Add<T>(this IEnumerable<T> items, T additionalItem)
        {
            return items == null ? new []{additionalItem} : items.Concat(new []{additionalItem});
        }
    }
}
