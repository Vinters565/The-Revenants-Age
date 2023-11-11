using System.Collections.Generic;
using System.Linq;

namespace Extension
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<TValue> SelectNotNullValues<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
            return enumerable
                .Select(x => x.Value)
                .Where(x => x != null);
        }
    }
}