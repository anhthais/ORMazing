using ORMazing.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ORMazing.Core.Models.Expressions
{
    public static class ExpressionHelper
    {
        public static string HandleBinaryExpression<T>(BinaryExpression binaryExpression) where T: class
        {
            var leftType = GetExpressionType(binaryExpression.Left);
            var rightType = GetExpressionType(binaryExpression.Right);
            var @operator = GetSqlOperator(binaryExpression.NodeType);
            if (!IsValidOperatorForTypes(@operator, leftType, rightType))
            {
                throw new ArgumentException($"Operator '{@operator}' is not valid for types {leftType} and {rightType}");
            }

            var left = GetExpressionValue<T>(binaryExpression.Left);
            var right = GetExpressionValue<T>(binaryExpression.Right);

            return $"{left} {@operator} {right}";
        }

        public static string GetExpressionValue<T>(Expression expression) where T : class
        {
            if (expression is MemberExpression memberExpression)
            {
                var propertyName = memberExpression.Member.Name;
                return AttributeHelper.GetColumnName<T>(propertyName);
            }
            else if (expression is ConstantExpression constantExpression)
            {
                return FormatValue(constantExpression.Value);
            }
            else if (expression is BinaryExpression binaryExpression)
            {
                return HandleBinaryExpression<T>(binaryExpression);
            }
            throw new ArgumentException($"Unsupported expression type: {expression.GetType()}");
        }

        public static string GetSqlOperator(ExpressionType expressionType)
        {
            return expressionType switch
            {
                ExpressionType.Add => "+",
                ExpressionType.Subtract => "-",
                ExpressionType.Multiply => "*",
                ExpressionType.Divide => "/",
                _ => throw new ArgumentException($"Unsupported binary operator: {expressionType}")
            };
        }

        public static string FormatValue(object? value)
        {
            return value switch
            {
                string s => $"'{s.Replace("'", "''")}'",
                DateTime dt => $"'{dt:yyyy-MM-dd HH:mm:ss}'",
                bool b => b ? "1" : "0",
                _ => value?.ToString() ?? "NULL"
            };
        }

        public static Type GetExpressionType(Expression expression)
        {
            return expression switch
            {
                MemberExpression memberExpression => memberExpression.Type,
                ConstantExpression constantExpression => constantExpression.Type,
                BinaryExpression binaryExpression => typeof(object),
                UnaryExpression unaryExpression => unaryExpression.Type,
                _ => throw new ArgumentException($"Unsupported expression type: {expression.GetType()}")
            };
        }

        public static bool IsValidOperatorForTypes(string @operator, Type leftType, Type rightType)
        {
            bool isLeftNumeric = IsNumericType(leftType);
            bool isRightNumeric = IsNumericType(rightType);
            return @operator switch
            {
                "+" => isLeftNumeric || leftType == typeof(string),
                "-" or "*" or "/" => isLeftNumeric && isRightNumeric,
                _ => false
            };
        }

        public static bool IsNumericType(Type type)
        {
            return type == typeof(int) ||
                   type == typeof(long) ||
                   type == typeof(float) ||
                   type == typeof(double) ||
                   type == typeof(decimal);
        }
    }
}
