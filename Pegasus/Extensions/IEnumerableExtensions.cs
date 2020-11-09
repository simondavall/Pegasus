using System.Collections.Generic;
using System.Linq;

namespace Pegasus.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Paginated<T>(this IEnumerable<T> list, int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;
            return list.Skip(skip).Take(pageSize);
        }
    }
}
