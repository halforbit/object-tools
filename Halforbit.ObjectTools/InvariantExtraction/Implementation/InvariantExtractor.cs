using Halforbit.ObjectTools.Collections;
using Halforbit.ObjectTools.Extensions;
using Halforbit.ObjectTools.InvariantExtraction.Interface;
using Halforbit.ObjectTools.ObjectBuild.Implementation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Halforbit.ObjectTools.InvariantExtraction.Implementation
{
    public class InvariantExtractor : IInvariantExtractor
    {
        public TObject ExtractInvariants<TObject>(
            Expression<Func<TObject, bool>> inputExpression,
            out Expression<Func<TObject, bool>> invariantExpression,
            TObject cloneSource = default(TObject))
        {
            var visitor = new InvariantFindingExpressionVisitor<TObject>(cloneSource);

            invariantExpression = visitor
                .Visit(inputExpression)
                as Expression<Func<TObject, bool>>;

            if (visitor.MemberValues.Count > 0 || typeof(TObject).IsClass)
            {
                var invariantObjectBuilder = cloneSource.IsDefaultValue() ?
                    new Builder<TObject>() :
                    new Builder<TObject>(cloneSource);

                foreach (var memberValue in visitor.MemberValues)
                {
                    var propertyInfo = memberValue.Item1;

                    var value = memberValue.Item2;

                    invariantObjectBuilder.Set(propertyInfo.Name, value);
                }

                return invariantObjectBuilder.Build();
            }
            else if (visitor.ThisValue != null)
            {
                return (TObject)visitor.ThisValue;
            }

            return default(TObject);
        }

        public IReadOnlyDictionary<string, object> ExtractInvariantDictionary<TObject>(
            Expression<Func<TObject, bool>> inputExpression,
            out Expression<Func<TObject, bool>> invariantExpression)
        {
            var visitor = new InvariantFindingExpressionVisitor<TObject>();

            invariantExpression = visitor
                .Visit(inputExpression)
                as Expression<Func<TObject, bool>>;

            var memberValueCount = visitor.MemberValues.Count;

            if (memberValueCount > 0 || typeof(TObject).IsClass)
            {
                var invariants = new Dictionary<string, object>(memberValueCount);

                foreach (var memberValue in visitor.MemberValues)
                {
                    var propertyInfo = memberValue.Item1;

                    var value = memberValue.Item2;

                    invariants[propertyInfo.Name] = value;
                }

                return invariants;
            }
            else if (visitor.ThisValue != null)
            {
                return new Dictionary<string, object>
                {
                    ["this"] = visitor.ThisValue
                };
            }

            return EmptyReadOnlyDictionary<string, object>.Instance;
        }
    }
}
