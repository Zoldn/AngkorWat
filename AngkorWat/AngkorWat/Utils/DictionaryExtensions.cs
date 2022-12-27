using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Utils
{
    internal static class DictionaryExtensions
    {
        public static void Deconstruct<T1, T2>(this IGrouping<T1, T2> grouping,
            out T1 key,
            out IEnumerable<T2> values)
        {
            key = grouping.Key;
            values = grouping;
        }
    }
}
