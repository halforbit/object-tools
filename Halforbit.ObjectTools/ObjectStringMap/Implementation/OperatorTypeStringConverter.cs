using System;
using System.Linq;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace Halforbit.ObjectTools.ObjectStringMap.Implementation
{
    static class OperatorTypeStringConverter<TValue>
    {
        static Func<TValue, string> _toString;

        static Func<string, TValue> _fromString;

        static OperatorTypeStringConverter()
        {
            var type = typeof(TValue);

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);

            var toStringMethod = methods.SingleOrDefault(m =>
                (m.Name == "op_Explicit" || m.Name == "op_Implicit") &&
                m.GetParameters().Single().ParameterType.Equals(type) &&
                m.ReturnType.Equals(typeof(string)));

            if (toStringMethod != null)
            {
                var p = Parameter(type, "o");

                var e = Lambda(Call(toStringMethod, p), p);

                _toString = e.Compile() as Func<TValue, string>;
            }

            var fromStringMethod = methods.SingleOrDefault(m =>
                (m.Name == "op_Explicit" || m.Name == "op_Implicit") &&
                m.GetParameters().Single().ParameterType.Equals(typeof(string)) &&
                m.ReturnType.Equals(type));

            if (fromStringMethod != null)
            {
                var p = Parameter(typeof(string), "s");

                var e = Lambda(Call(fromStringMethod, p), p);

                _fromString = e.Compile() as Func<string, TValue>;
            }
        }

        public static bool IsOperatorConvertable => _fromString != null && _toString != null;

        public static string TypeToString(TValue type)
        {
            return _toString?.Invoke(type);
        }

        public static TValue StringToType(string stringValue)
        {
            return _fromString != null ? _fromString(stringValue) : default;
        }
    }
}
