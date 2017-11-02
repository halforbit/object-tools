using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Halforbit.ObjectTools.Collections
{
    public class EmptyReadOnlyDictionary<TKey, TValue> :
        IReadOnlyDictionary<TKey, TValue>
    {
        static readonly EmptyReadOnlyDictionary<TKey, TValue> _instance =
            new EmptyReadOnlyDictionary<TKey, TValue>();

        public static EmptyReadOnlyDictionary<TKey, TValue> Instance => _instance;

        public TValue this[TKey key]
        {
            get { throw new KeyNotFoundException(); }
        }

        public int Count => 0;

        public IEnumerable<TKey> Keys => Enumerable.Empty<TKey>();

        public IEnumerable<TValue> Values => Enumerable.Empty<TValue>();

        public bool ContainsKey(TKey key) => false;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
            Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
