using Halforbit.ObjectTools.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Halforbit.ObjectTools.Validation
{
    public class ValidationErrors : IReadOnlyList<ValidationError>
    {
        internal readonly IReadOnlyList<ValidationError> _validationErrors;

        static ValidationErrors()
        {
            Empty = new ValidationErrors(new ValidationError[] { });
        }

        internal ValidationErrors(IReadOnlyList<ValidationError> validationErrors)
        {
            _validationErrors = validationErrors;
        }

        public static ValidationErrors Empty { get; private set; }

        public int Count => _validationErrors.Count;

        public ValidationError this[int index] => _validationErrors[index];

        public IEnumerator<ValidationError> GetEnumerator() => _validationErrors.GetEnumerator();

        public override string ToString() => string.Join("\r\n", _validationErrors);

        IEnumerator IEnumerable.GetEnumerator() => _validationErrors.GetEnumerator();

        public ValidationErrors<TItem> Regarding<TItem>(TItem item) => new ValidationErrors<TItem>(item, _validationErrors);
    }

    public class ValidationErrors<TItem> : ValidationErrors
    {
        internal ValidationErrors(
            TItem item, 
            IReadOnlyList<ValidationError> validationErrors) : base(validationErrors) 
        {
            Item = item;
        }

        public TItem Item { get; }
    }

    public static class ValidationErrorsExtensions
    {
        public static ValidationErrors With(
            this ValidationErrors source,
            ValidationError validationError)
        {
            var validationErrors = new List<ValidationError>(source.Count + 1);

            validationErrors.AddRange(source);

            validationErrors.Add(validationError);

            return new ValidationErrors(validationErrors);
        }

        public static ValidationErrors With(
            this ValidationErrors source,
            IReadOnlyList<ValidationError> validationErrors)
        {
            var newValidationErrors = new List<ValidationError>(source.Count + validationErrors.Count);

            newValidationErrors.AddRange(source);

            newValidationErrors.AddRange(validationErrors);

            return new ValidationErrors(newValidationErrors);
        }

        public static ValidationErrors<TItem> With<TItem>(
            this ValidationErrors<TItem> source,
            ValidationError validationError)
        {
            var validationErrors = new List<ValidationError>(source.Count + 1);

            validationErrors.AddRange(source);

            validationErrors.Add(validationError);

            return new ValidationErrors<TItem>(source.Item, validationErrors);
        }

        public static ValidationErrors<TItem> With<TItem>(
            this ValidationErrors<TItem> source,
            IReadOnlyList<ValidationError> validationErrors)
        {
            var newValidationErrors = new List<ValidationError>(source.Count + validationErrors.Count);

            newValidationErrors.AddRange(source);

            newValidationErrors.AddRange(validationErrors);

            return new ValidationErrors<TItem>(source.Item, newValidationErrors);
        }

        public static ValidationErrors If(
            this ValidationErrors source,
            bool predicate,
            ValidationError validationError)
        {
            return predicate ?
                source.With(validationError) :
                source;
        }

        public static ValidationErrors<TItem> If<TItem>(
            this ValidationErrors<TItem> source,
            bool predicate,
            ValidationError validationError)
        {
            return predicate ?
                source.With(validationError) :
                source;
        }

        public static ValidationErrors Require<TValue>(
            this ValidationErrors source,
            TValue value,
            string name)
        {
            return ValueIsMissing(value) ?
                source.With(ValidationError.Required(name)) :
                source;
        }

        static bool ValueIsMissing<TValue>(TValue value)
        {
            var valueType = typeof(TValue);

            var enumerableInterface = valueType
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>));

            var missing = false;

            if (value is string str)
            {
                missing = string.IsNullOrWhiteSpace(str);
            }
            else if (valueType == typeof(Guid?))
            {
                var g = (Guid?)(object)value;

                missing = !g.HasValue || g.Value == Guid.Empty;
            }
            else if (value == null)
            {
                missing = true;
            }
            //else if (enumerableInterface != null)
            //{
            //    missing = (int)valueType.GetProperty(nameof(IReadOnlyCollection<object>.Count)).GetValue(value) == 0;
            //}
            else
            {
                missing = value.IsDefaultValue();
            }

            return missing;
        }

        public static ValidationErrors<TItem> Require<TItem, TValue>(
            this ValidationErrors<TItem> source,
            Expression<Func<TItem, TValue>> propertySelector)
        {
            var value = propertySelector.Compile()(source.Item);

            var valueType = typeof(TValue);

            if (IsAnonymousType(valueType))
            {
                foreach (var property in valueType.GetProperties())
                {
                    var propertyValue = property.GetValue(value);

                    var propertyMissing = (bool)typeof(ValidationErrorsExtensions)
                        .GetMethod(nameof(ValueIsMissing), BindingFlags.NonPublic | BindingFlags.Static)
                        .MakeGenericMethod(property.PropertyType)
                        .Invoke(default(object), new object[] { propertyValue });

                    if (propertyMissing)
                    {
                        source = source.With($"{property.Name} required.");
                    }
                }
            }
            else if (ValueIsMissing(value))
            {
                var propertyInfo = GetPropertyInfo(propertySelector);

                return source.With($"{propertyInfo.Name} required.");
            }

            return source;
        }

        static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member;

            if (propertyLambda.Body is UnaryExpression u)
            {
                member = u.Operand as MemberExpression;
            }
            else
            {
                member = propertyLambda.Body as MemberExpression;
            }

            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

        static bool IsAnonymousType(Type type) => Attribute.IsDefined(
            type, typeof(CompilerGeneratedAttribute), false) && 
                type.IsGenericType && 
                type.Name.Contains("AnonymousType") && 
                (type.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase) ||
                    type.Name.StartsWith("VB$", StringComparison.OrdinalIgnoreCase)) && 
                (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
    }
}
