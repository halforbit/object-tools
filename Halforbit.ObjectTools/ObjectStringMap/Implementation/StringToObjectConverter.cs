using System;
using System.Linq;
using System.Reflection;

namespace Halforbit.ObjectTools.ObjectStringMap.Implementation
{
    static class StringToObjectConverter
    {
        public static object ConvertToObject(
            Type type,
            string stringValue,
            string format)
        {
            var typeInfo = type.GetTypeInfo();

            try
            {
                if (type.Equals(typeof(Guid)) || type.Equals(typeof(Guid?)))
                {
                    return Guid.Parse(stringValue);
                }
                else if ((type.Equals(typeof(DateTime)) || type.Equals(typeof(DateTime?))) &&
                    !string.IsNullOrWhiteSpace(format))
                {
                    return DateTime.ParseExact(stringValue, format, null);
                }
                else
                {
                    if (typeInfo.IsGenericType &&
                        type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        type = typeInfo.GenericTypeArguments[0];
                    }

                    if (typeInfo.IsEnum)
                    {
                        if (string.IsNullOrWhiteSpace(format) || format.Equals("t", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return Enum.Parse(type, stringValue.TrainToPascalCase());
                        }
                        else if (format.Equals("p", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return Enum.Parse(type, stringValue);
                        }
                        else if (format.Equals("c", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return Enum.Parse(type, stringValue.CamelToPascalCase());
                        }
                        else if (format.Equals("i", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return Enum.Parse(type, stringValue);
                        }
                        else
                        {
                            throw new ArgumentException($"Unrecognized format string `{format}`, should be one of `c`, `p`, `c`, `i`");
                        }
                    }
                    else 
                    {
                        var op = type
                            .GetMethods(BindingFlags.Public | BindingFlags.Static)
                            .FirstOrDefault(m => (m.Name == "op_Explicit" || m.Name == "op_Implicit") &&
                                m.ReturnType.Equals(type) &&
                                m.GetParameters().Length == 1 &&
                                m.GetParameters().Single().ParameterType.Equals(typeof(string)));

                        if (op != null)
                        {
                            return op.Invoke(null, new object[] { stringValue });
                        }
                        else
                        {
                            return Convert.ChangeType(stringValue, type);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
