using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Halforbit.ObjectTools.DeferredConstruction
{
    public static class ConstructableExtensions
    {
        public static Constructable Type(
            this Constructable constructable,
            Type type)
        {
            return new Constructable(
                type: type,
                typeArguments: constructable?.TypeArguments,
                arguments: constructable?.Arguments,
                value: constructable?.Value);
        }

        public static Constructable TypeArguments(
            this Constructable constructable,
            params Type[] typeArguments)
        {
            return new Constructable(
                type: constructable.Type,
                typeArguments: typeArguments,
                arguments: constructable.Arguments,
                value: constructable.Value);
        }

        public static Constructable Argument(
            this Constructable constructable,
            string name,
            Constructable argumentValue)
        {
            return new Constructable(
                type: constructable.Type,
                typeArguments: constructable.TypeArguments,
                arguments: constructable.Arguments.Set(name, argumentValue),
                value: constructable.Value);
        }

        public static Constructable Argument(
            this Constructable constructable,
            string name,
            object argumentValue)
        {
            return new Constructable(
                type: constructable.Type,
                typeArguments: constructable.TypeArguments,
                arguments: constructable.Arguments.Set(name, new Constructable(
                    type: null,
                    typeArguments: null,
                    arguments: null,
                    value: argumentValue)),
                value: constructable.Value);
        }

        public static Constructable Argument(
            this Constructable constructable,
            string name,
            Func<Constructable, Constructable> getArgumentValue)
        {
            return new Constructable(
                type: constructable.Type,
                typeArguments: constructable.TypeArguments,
                arguments: constructable.Arguments.Set(name, getArgumentValue(constructable.Arguments[name])),
                value: constructable.Value);
        }

        public static Constructable ArgumentNull(
            this Constructable constructable,
            string name)
        {
            return new Constructable(
                type: constructable.Type,
                typeArguments: constructable.TypeArguments,
                arguments: constructable.Arguments.Set(name, null),
                value: constructable.Value);
        }

        public static object Construct(this Constructable constructable)
        {
            if (constructable.Value != null) return constructable.Value;

            var type = constructable.Type;

            if (type == null) throw new ArgumentNullException("Type");

            if (constructable.TypeArguments.Any())
            {
                type = type.MakeGenericType(constructable.TypeArguments.ToArray());
            }
            else if (type.IsGenericTypeDefinition)
            {
                throw new ArgumentException($"Type {type.Name} requires generic type arguments but none were specified.");
            }

            var constructors = type.GetConstructors(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var constructorCount = constructors.Count();

            if (constructorCount == 0) throw new ArgumentException($"Type `{type.Name}` does not have a constructor");

            if (constructorCount > 1) throw new ArgumentException($"Type `{type.Name}` has more than one constructor");

            var constructor = constructors.Single();

            var arguments = ResolveArguments(
                type.Name,
                constructor.GetParameters(),
                constructable.Arguments);

            var instance = Activator.CreateInstance(type, arguments);

            return instance;
        }

        static object[] ResolveArguments(
            string typeName,
            IReadOnlyList<ParameterInfo> parameters,
            IReadOnlyDictionary<string, Constructable> argumentConstructables)
        {
            var arguments = new object[parameters.Count];

            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];

                if (!argumentConstructables.TryGetValue(parameter.Name, out var argumentConstructable))
                {
                    throw new ArgumentException(
                        $"Constructor for type `{typeName}` requires parameter `{parameter.Name}` " +
                        $"but it was not provided");
                }

                if (argumentConstructable != null)
                {
                    var argumentValue = argumentConstructable.Construct();

                    if (argumentValue != null)
                    {
                        if (!parameter.ParameterType.IsAssignableFrom(argumentValue.GetType()))
                        {
                            throw new ArgumentException(
                                $"Constructor parameter `{parameter.Name}` for type `{typeName}` " +
                                $"is of type `{parameter.ParameterType.Name}` but an argument " +
                                $"of type `{argumentConstructable.GetType().Name}` was provided");
                        }

                        arguments[i] = argumentValue;
                    }
                }
            }

            return arguments;
        }

        public static IReadOnlyDictionary<TKey, TValue> Set<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> source,
            TKey key,
            TValue value)
        {
            var dictionary = source.ToDictionary(kv => kv.Key, kv => kv.Value);

            dictionary[key] = value;

            return dictionary;
        }
    }
}
