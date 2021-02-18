using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace SearchStory.App.Shared
{
    public static class UIExtensions
    {

        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }
    }
}