using System;
using System.Collections.Generic;
using System.Linq;

namespace Halforbit.ObjectTools.ObjectStringMap.Implementation
{
    static class ObjectToStringConverter
    {
        public const string ThisKeyword = "this";

        public static string ResolveStringFromKeyValue(
            IReadOnlyDictionary<string, object> objMembers,
            string name,
            string format)
        {
            if (objMembers.TryGetValue(name, out var value))
            {
                return FormatValue(value, format);
            }
            else
            {
                return null;
            }
        }

        public static string ResolveStringFromProperty(
            object obj,
            string name,
            string format)
        {
            var value = ResolvePropertyValue(obj, name);

            return FormatValue(value, format);
        }

        static object ResolvePropertyValue(
            object obj,
            string name)
        {
            var type = obj.GetType();

            if (name == ThisKeyword)
            {
                return obj;
            }
            else
            {
                var property = type
                    .GetProperties()
                    .SingleOrDefault(p => p.Name == name);

                if (property != null)
                {
                    return property.GetValue(obj);
                }
                else
                {
                    var field = type.GetFields().SingleOrDefault(p => p.Name == name);

                    if (field != null)
                    {
                        return field.GetValue(obj);
                    }
                    else
                    {
                        throw new Exception($"Could not resolve string map property or field '{name}'");
                    }
                }
            }
        }

        static string FormatValue(
            object value,
            string format)
        {
            if (value == null)
            {
                return null;
            }

            if (value.GetType().IsEnum)
            {
                if (string.IsNullOrWhiteSpace(format) || format.Equals("t", StringComparison.InvariantCultureIgnoreCase))
                {
                    return value.ToString().PascalToTrainCase();
                }
                else if (format.Equals("p", StringComparison.InvariantCultureIgnoreCase))
                {
                    return value.ToString();
                }
                else if (format.Equals("c", StringComparison.InvariantCultureIgnoreCase))
                {
                    return value.ToString().PascalToCamelCase();
                }
                else if (format.Equals("i", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Convert.ChangeType(value, ((Enum)value).GetTypeCode()).ToString();
                }
                else
                {
                    throw new ArgumentException($"Unrecognized format string `{format}`, should be one of `c`, `p`, `c`, `i`");
                }
            }

            if (!string.IsNullOrWhiteSpace(format))
            {
                var formattable = value as IFormattable;

                if (formattable == null)
                {
                    throw new ArgumentException($"Injection point has a format but is not IFormattable.");
                }

                return formattable.ToString(format, null);
            }

            if (value is Guid)
            {
                return ((Guid)value).ToString("N");
            }

            return value.ToString();
        }
    }
}
