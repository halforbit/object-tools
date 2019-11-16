using System;
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

                    return Convert.ChangeType(stringValue, type);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
