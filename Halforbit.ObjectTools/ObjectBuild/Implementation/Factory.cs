using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Halforbit.ObjectTools.ObjectBuild.Implementation
{
    public static class Factory<TObject>
    {
        static Factory()
        {
            Build = BuildFactoryFunc();
        }

        public static Func<ConcurrentDictionary<string, object>, TObject, TObject> Build { get; }

        static Func<ConcurrentDictionary<string, object>, TObject, TObject> BuildFactoryFunc()
        {
            var typeInfo = typeof(TObject).GetTypeInfo();

            var constructors = typeInfo
                .DeclaredConstructors
                .Select(ci => new
                {
                    ConstructorInfo = ci,

                    Parameters = ci.GetParameters()
                })
                .ToList();

            var constructor = constructors
                .OrderByDescending(c => c.Parameters.Count())
                .First();

            var arguments = new List<Expression>();

            var dictionaryParameter = Expression.Parameter(
                typeof(ConcurrentDictionary<string, object>),
                "d");

            var sourceParameter = Expression.Parameter(
                typeof(TObject),
                "s");

            var concurrentDictionaryType = typeof(ConcurrentDictionary<string, object>).GetTypeInfo();

            var getOrAddMethodInfo = concurrentDictionaryType
                .DeclaredMethods
                .Single(m => m.Name == "GetOrAdd" && 
                    m.GetParameters().Skip(1).First().ParameterType == typeof(object));

            var properties = typeInfo.GetProperties()
                .ToDictionary(p => p.Name.ToLower(), p => p);

            var propertiesToSet = properties
                .Where(p => p.Value.SetMethod != null)
                .ToDictionary(p => p.Key, p => p.Value);

            var fields = typeInfo.GetFields()
                .ToDictionary(f => f.Name.ToLower(), f => f);

            foreach (var parameter in constructor.Parameters)
            {
                var key = parameter.Name.ToLower();

                var parameterType = parameter.ParameterType;

                if(properties.Any())
                {
                    var property = properties[key];

                    arguments.Add(BuildPropertyArgumentExpression(
                        dictionaryParameter,
                        sourceParameter,
                        getOrAddMethodInfo,
                        key,
                        property,
                        parameterType));
                }
                else
                {
                    arguments.Add(BuildFieldArgumentExpression(
                        dictionaryParameter,
                        sourceParameter,
                        getOrAddMethodInfo,
                        key,
                        fields[key],
                        parameterType));
                }

                propertiesToSet.Remove(key);
            }

            var newExpression = Expression.New(
                constructor.ConstructorInfo,
                arguments);

            var outerExpression = newExpression as Expression;

            if (propertiesToSet.Any())
            {
                var memberBindings = propertiesToSet
                    .Select(p => Expression.Bind(
                        p.Value,
                        BuildPropertyArgumentExpression(
                            dictionaryParameter,
                            sourceParameter,
                            getOrAddMethodInfo,
                            p.Key,
                            p.Value,
                            p.Value.PropertyType)))
                    .ToList();

                outerExpression = Expression.MemberInit(newExpression, memberBindings);
            }

            var lambda = Expression.Lambda<Func<ConcurrentDictionary<string, object>, TObject, TObject>>(
                outerExpression,
                dictionaryParameter,
                sourceParameter);

            return lambda.Compile();
        }

        static UnaryExpression BuildPropertyArgumentExpression(
            ParameterExpression dictionaryParameter,
            ParameterExpression sourceParameter,
            MethodInfo getOrAddMethodInfo,
            string key,
            PropertyInfo propertyInfo,
            Type parameterType)
        {
            var propertyType = propertyInfo.PropertyType;

            return Expression.Convert(
                Expression.Call(
                    dictionaryParameter,
                    getOrAddMethodInfo,
                    Expression.Constant(key),
                    Expression.TypeAs(
                        Expression.Condition(
                            Expression.Equal(
                                sourceParameter,
                                Expression.Constant(default(TObject))),
                            Expression.Default(propertyType),
                            Expression.Property(
                                sourceParameter,
                                propertyInfo)),
                        typeof(object))),
                parameterType);
        }

        static UnaryExpression BuildFieldArgumentExpression(
            ParameterExpression dictionaryParameter,
            ParameterExpression sourceParameter,
            MethodInfo getOrAddMethodInfo,
            string key,
            FieldInfo fieldInfo,
            Type parameterType)
        {
            return Expression.Convert(
                Expression.Call(
                    dictionaryParameter,
                    getOrAddMethodInfo,
                    Expression.Constant(key),
                    Expression.TypeAs(
                        Expression.Field(
                            sourceParameter,
                            fieldInfo),
                        typeof(object))),
                parameterType);
        }
    }
}
