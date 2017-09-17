using System;
using System.Reflection;

namespace Halforbit.ObjectTools.Extensions
{
    public static class DefaultValueExtensions
    {
        public static bool IsDefaultValue<TValue>(this TValue argument)
        {
            // Normal scenarios

            if (argument == null) return true;

            if (Equals(argument, default(TValue))) return true;

            // Non-null nullables

            var methodType = typeof(TValue);

            if (Nullable.GetUnderlyingType(methodType) != null) return false;

            // Boxed value types

            var argumentType = argument.GetType();
                
            var argumentTypeInfo = argumentType.GetTypeInfo();

            if (argumentTypeInfo.IsValueType && argumentType != methodType)
            {
                var obj = Activator.CreateInstance(argument.GetType());

                return obj.Equals(argument);
            }

            return false;
        }
    }
}
