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
        //public static string ConvertExpressionToSql(Expression expression)
        //{
        //    if (expression is BinaryExpression binaryExpr)
        //    {
        //        var left = ConvertExpressionToSql(binaryExpr.Left);
        //        var right = ConvertExpressionToSql(binaryExpr.Right);
        //        var @operator = GetSqlOperator(binaryExpr.NodeType);

        //        return $"{left} {@operator} {right}";
        //    }
        //    else if (expression is MemberExpression memberExpr)
        //    {
        //        return AttributeHelper.GetColumnName(memberExpr.Member);
        //    }
        //    else if (expression is ConstantExpression constantExpr)
        //    {
        //        return FormatSqlValue(constantExpr.Value);
        //    }
        //    else if (expression is MethodCallExpression methodCall)
        //    {
        //        return ConvertMethodCallToSql(methodCall);
        //    }

        //    throw new ArgumentException($"Unsupported expression type: {expression.GetType()}");
        //}

        //public static string ExtractColumnList(Expression expression)
        //{
        //    if (expression is NewExpression newExpr)
        //    {
        //        var columns = newExpr.Arguments
        //            .Select(arg =>
        //            {
        //                if (arg is MemberExpression memberExpr)
        //                {
        //                    return AttributeHelper.GetColumnName(memberExpr.Member);
        //                }

        //                throw new ArgumentException($"Unsupported expression in GroupBy: {arg.GetType()}");
        //            });

        //        return string.Join(", ", columns);
        //    }
        //    else if (expression is MemberExpression singleColumnExpr)
        //    {
        //        return AttributeHelper.GetColumnName(singleColumnExpr.Member);
        //    }

        //    throw new ArgumentException("Invalid GroupBy expression.");
        //}

        //private static string GetSqlOperator(ExpressionType nodeType)
        //{
        //    return nodeType switch
        //    {
        //        ExpressionType.Equal => "=",
        //        ExpressionType.NotEqual => "<>",
        //        ExpressionType.GreaterThan => ">",
        //        ExpressionType.GreaterThanOrEqual => ">=",
        //        ExpressionType.LessThan => "<",
        //        ExpressionType.LessThanOrEqual => "<=",
        //        ExpressionType.AndAlso => "AND",
        //        ExpressionType.OrElse => "OR",
        //        _ => throw new ArgumentException($"Unsupported binary operator: {nodeType}")
        //    };
        //}

        //private static string FormatSqlValue(object? value)
        //{
        //    return value switch
        //    {
        //        null => "NULL",
        //        string str => $"'{str.Replace("'", "''")}'",
        //        DateTime dt => $"'{dt:yyyy-MM-dd HH:mm:ss}'",
        //        _ => value.ToString() ?? "NULL"
        //    };
        //}

        //private static string ConvertMethodCallToSql(MethodCallExpression methodCall)
        //{
        //    if (methodCall.Method.DeclaringType == typeof(SqlFunctions))
        //    {
        //        var methodName = methodCall.Method.Name.ToUpper();
        //        var arguments = methodCall.Arguments.Select(ConvertExpressionToSql);

        //        return $"{methodName}({string.Join(", ", arguments)})";
        //    }

        //    throw new ArgumentException($"Unsupported method call: {methodCall.Method.Name}");
        //}
    
    }
}
