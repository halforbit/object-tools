using Halforbit.ObjectTools.Extensions;
using System;
using System.Collections.Generic;

namespace Halforbit.ObjectTools.DeferredConstruction
{
    public class Constructable
    {
        public Constructable(
            Type type,
            IReadOnlyList<Type> typeArguments,
            IReadOnlyDictionary<string, Constructable> arguments,
            object value)
        {
            Type = type;

            TypeArguments = typeArguments.OrEmptyReadOnlyListIfDefault();

            Arguments = arguments.OrEmptyReadOnlyDictionaryIfDefault();

            Value = value;
        }

        public Type Type { get; }

        public IReadOnlyList<Type> TypeArguments { get; }

        public IReadOnlyDictionary<string, Constructable> Arguments { get; }

        public object Value { get; }
    }
}
