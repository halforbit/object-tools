using System.Collections.Generic;

namespace Halforbit.ObjectTools.Extensions
{
    public static class StringExtensions
    {
        public static string JoinString<TValue>(
            this IEnumerable<TValue> values, 
            string separator)
        {
            return string.Join(separator, values);
        }
    }
}
