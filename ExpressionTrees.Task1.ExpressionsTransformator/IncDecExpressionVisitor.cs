using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionTrees.Task1.ExpressionsTransformer
{

    public class IncDecExpressionVisitor : ExpressionVisitor
    {
        private Dictionary<string, double> _replacements;

        public Expression Transform(Expression expression, Dictionary<string, double> replacements = null)
        {
            _replacements = replacements;
            return Visit(expression);
        }

        protected override Expression VisitBinary(BinaryExpression node)
            => node.Right is ConstantExpression c
               && IsNumericType(c.Type)
               && Convert.ToInt32(c.Value) == 1
               && (node.NodeType == ExpressionType.Add || node.NodeType == ExpressionType.Subtract)
               && node.Left.NodeType == ExpressionType.Parameter
               && !IsParameterReplaced((node.Left as ParameterExpression).Name)
                ? node.NodeType == ExpressionType.Add
                    ? Expression.Increment(node.Left)
                    : Expression.Decrement(node.Left)
                : base.VisitBinary(node);

        protected override Expression VisitLambda<T>(Expression<T> node)
            => Expression.Lambda(typeof(T), Visit(node.Body), node.Parameters);

        protected override Expression VisitParameter(ParameterExpression node)
            => IsParameterReplaced(node.Name)
                ? Expression.Constant(_replacements[node.Name], node.Type)
                : base.VisitParameter(node);

        private bool IsParameterReplaced(string parameterName)
            => _replacements?.ContainsKey(parameterName) ?? false;

        private static bool IsNumericType(Type type)
            => NumericTypes.Contains(Type.GetTypeCode(type));

        private static readonly TypeCode[] NumericTypes =
        {
            TypeCode.Byte, TypeCode.SByte, TypeCode.UInt16, TypeCode.UInt32, TypeCode.UInt64, TypeCode.Int16,
            TypeCode.Int32, TypeCode.Int64, TypeCode.Decimal, TypeCode.Double, TypeCode.Single
        };
    }
}
