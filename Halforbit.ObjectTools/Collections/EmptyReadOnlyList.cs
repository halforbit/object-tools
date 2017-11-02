using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Halforbit.ObjectTools.Collections
{
    public class EmptyReadOnlyList<TValue> : IReadOnlyList<TValue>
    {
        static EmptyReadOnlyList<TValue> _instance = new EmptyReadOnlyList<TValue>();

        public static EmptyReadOnlyList<TValue> Instance => _instance;

        public TValue this[int index]
        {
            get
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public int Count => 0;

        public IEnumerator<TValue> GetEnumerator()
        {
            return Enumerable.Empty<TValue>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
