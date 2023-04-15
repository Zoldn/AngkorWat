using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Utils
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Apply<T>(this IEnumerable<T> x, Action<T> action)
        {
            foreach (var item in x)
            {
                action(item);
            }

            return x;
        }

        public static T ArgMax<T>(this IEnumerable<T> x, Func<T, double> selector)
        {
            if (!x.Any())
            {
                throw new ArgumentOutOfRangeException();
            }

            var ret = x.First();

            foreach (var item in x)
            {
                if (selector(item) > selector(ret))
                {
                    ret = item;
                }
            }

            return ret;
        }

        public static T ArgMin<T>(this IEnumerable<T> x, Func<T, double> selector)
        {
            if (!x.Any())
            {
                throw new ArgumentOutOfRangeException();
            }

            var ret = x.First();

            foreach (var item in x)
            {
                if (selector(item) < selector(ret))
                {
                    ret = item;
                }
            }

            return ret;
        }

        public static Dictionary<TGroupingKey, double> Summarise<T, TGroupingKey>(this IEnumerable<T> x,
            Func<T, TGroupingKey> keySelector,
            Func<IEnumerable<T>, double> aggregator)
            where TGroupingKey : notnull
        {
            var t = x
                .GroupBy(e => keySelector(e))
                .ToDictionary(
                    g => g.Key,
                    g => aggregator(g)
                    );

            return t;
        }

        public static double Average<T>(this IEnumerable<T> x, 
            Func<T, double> selector, Func<T, double> weightSelector)
        {
            if (!x.Any())
            {
                throw new ArgumentOutOfRangeException();
            }

            var d1 = x.Sum(x => weightSelector(x) * selector(x));
            var d2 = x.Sum(x => weightSelector(x));

            if (d2 == 0.0d)
            {
                throw new DivideByZeroException();
            }

            return d1 / d2;
        }
    }
}
