﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Halforbit.ObjectTools.InvariantExtraction.Implementation
{
    class InvariantFindingExpressionVisitor<TObject> : ExpressionVisitor
    {
        List<Tuple<PropertyInfo, object>> _propertyValues = 
            new List<Tuple<PropertyInfo, object>>();

        readonly TObject _cloneSource;

        public InvariantFindingExpressionVisitor(
            TObject cloneSource = default(TObject))
        {
            _cloneSource = cloneSource;
        }

        public IReadOnlyList<Tuple<PropertyInfo, object>> MemberValues
        {
            get { return _propertyValues; }
        }

        public object ThisValue { get; private set; }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if(node.NodeType == ExpressionType.Equal)
            {
                // If this binary operator is an equals...

                if (node.Left.NodeType == ExpressionType.MemberAccess)
                {
                    // If the left side is a member access...

                    var memberExpression = node.Left as MemberExpression;

                    var propertyInfo = memberExpression.Member as PropertyInfo;

                    if(propertyInfo != null)
                    {
                        // If the member accessed is a property...

                        if (memberExpression.Expression.NodeType == ExpressionType.Parameter)
                        {
                            // If the expression of the member access is a lambda parameter...

                            var value = EvaluateExpressionValue(node.Right);

                            // Add the property's info and value to the list.

                            _propertyValues.Add(Tuple.Create(
                                propertyInfo,
                                value));

                            // Return a binary equals with a guaranteed constant value on the right.

                            return Expression.MakeBinary(
                                ExpressionType.Equal,
                                node.Left,
                                Expression.Constant(value, node.Right.Type));
                        }

                        if (memberExpression.Expression.NodeType == ExpressionType.Convert)
                        {
                            // If we're maybe a lambda parameter wrapped in a convert for whatever weird reason...

                            var convertExpression = memberExpression.Expression as UnaryExpression;

                            if (convertExpression.Operand.NodeType == ExpressionType.Parameter)
                            {
                                // If the expression of the member access is a lambda parameter...

                                var value = EvaluateExpressionValue(node.Right);

                                // Add the property's info and value to the list.

                                _propertyValues.Add(Tuple.Create(
                                    propertyInfo,
                                    value));

                                // Return a binary equals with a guaranteed constant value on the right.

                                return Expression.MakeBinary(
                                    ExpressionType.Equal,
                                    Expression.Property(convertExpression.Operand, propertyInfo),
                                    Expression.Constant(value, node.Right.Type));
                            }
                        }
                    }
                }
                else if (node.Left.NodeType == ExpressionType.Parameter)
                {
                    // Else if the left side is a lambda parameter...

                    var value = EvaluateExpressionValue(node.Right);

                    // Set the this value to the evaluated value.

                    ThisValue = value;

                    // Return a binary equals with a guaranteed constant value on the right.

                    return Expression.MakeBinary(
                        ExpressionType.Equal,
                        node.Left,
                        Expression.Constant(value, node.Right.Type));
                }
            }

            // Else business as usual.

            return base.VisitBinary(node);
        }

        object EvaluateExpressionValue(Expression valueExpression)
        {
            var parameter = new ParameterFindingExpressionVisitor().FindFirstParameter(valueExpression);

            var lambda = parameter == null ?
                Expression.Lambda(valueExpression) :
                Expression.Lambda(valueExpression, parameter);

            var compiled = lambda.Compile();

            return parameter == null ?
                compiled.DynamicInvoke() :
                compiled.DynamicInvoke(_cloneSource);
        }
    }
}
