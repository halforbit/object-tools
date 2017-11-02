using Halforbit.ObjectTools.Collections;
using System;
using System.Collections.Generic;

namespace Halforbit.ObjectTools.Extensions
{
    public static class ConstructionExtensions
    {
        public static bool IsDefaultValue<TKey>(this TKey key)
        {
            return EqualityComparer<TKey>.Default.Equals(key, default(TKey));
        }

        public static TKey OrIfDefault<TKey>(
            this TKey key,
            Func<TKey> getFallback)
        {
            return key.IsDefaultValue() ? getFallback() : key;
        }

        public static TKey OrIfDefault<TKey>(
            this TKey key,
            TKey fallback)
        {
            return key.IsDefaultValue() ? fallback : key;
        }

        public static string OrIfNullOrWhitespace(
            this string source,
            string fallback)
        {
            return string.IsNullOrWhiteSpace(source) ? fallback : source;
        }

        public static IReadOnlyList<TValue> OrEmptyReadOnlyListIfDefault<TValue>(
            this IReadOnlyList<TValue> source)
        {
            return source == null ? EmptyReadOnlyList<TValue>.Instance : source;
        }

        public static IReadOnlyDictionary<TKey, TValue> OrEmptyReadOnlyDictionaryIfDefault<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> source)
        {
            return source == null ? EmptyReadOnlyDictionary<TKey, TValue>.Instance : source;
        }

        public static string OrEmptyStringIfDefault(
            this string source)
        {
            return source == null ? string.Empty : source;
        }

        public static Guid OrNewGuidIfDefault(
            this Guid key)
        {
            return key.IsDefaultValue() ? Guid.NewGuid() : key;
        }

        public static bool IsEmptyOrNullGuid<TKey>(this TKey key)
        {
            if (typeof(Guid?).IsAssignableFrom(typeof(TKey)))
            {
                return ((key as Guid?) ?? default(Guid)) == default(Guid);
            }
            else if (key is Guid)
            {
                return ((Guid)(object)key) == default(Guid);
            }

            return false;
        }
    }
}
