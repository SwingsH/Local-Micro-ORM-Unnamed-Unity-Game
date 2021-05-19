using System.Collections.Generic;

namespace Tizsoft.Extensions
{
    public static class IndexExtensions
    {
        public static bool IsValidIndex<T>(this IEnumerable<T> container, int index)
        {
            if (container != null && 0 <= index)
            {
                // 避免使用 Linq
                var count = 0;
                foreach (var _ in container)
                {
                    ++count;
                }
                return index < count;
            }
            return false;
        }

        public static bool IsValidIndex<T>(this T[] container, int index)
        {
            return container != null && 0 <= index && index < container.Length;
        }

        public static bool IsValidIndex<T>(this ICollection<T> list, int index)
        {
            return list != null && 0 <= index && index < list.Count;
        }
    }
}
