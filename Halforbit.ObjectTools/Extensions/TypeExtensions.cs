using System;

namespace Halforbit.ObjectTools.Extensions
{
    public static class TypeExtensions
    {
        public static object Invoke(
            this object o,
            string methodName,
            params object[] args) => o.GetType().GetMethod(methodName).Invoke(o, args);

        public static object ConvertTo(this object value, Type targetType) =>
            targetType.IsEnum ?
                Enum.ToObject(targetType, value) :
                value != null ?
                    Convert.ChangeType(value, targetType) :
                    targetType.GetDefaultValue();

        public static object GetDefaultValue(this Type type) => type.IsValueType ?
            Activator.CreateInstance(type) :
            null;
    }
}
