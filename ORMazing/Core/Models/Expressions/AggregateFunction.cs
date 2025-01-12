using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ORMazing.Core.Models.Expressions
{
    public static class AggregateFunction
    {
        public static string Count<T>(Expression<Func<T, object>> column, string? alias = null) where T : class
        {
            var columnName = ExpressionHelper.GetColumnNameFromEntity(column);
            var aliasString = alias != null ? $" AS {alias}" : $"Count{columnName}";
            return $"COUNT({columnName}){aliasString}";
        }

        public static string Count(string column, string? alias = null)
        {
            var aliasString = alias != null ? $" AS {alias}" : $"Count{column}";
            return $"COUNT({column}){aliasString}";
        }

        public static string Sum<T>(Expression<Func<T, object>> column, string? alias = null) where T : class
        {
            var columnName = ExpressionHelper.GetColumnNameFromEntity(column);
            var aliasString = alias != null ? $" AS {alias}" : $"Sum{columnName}";
            return $"SUM({columnName}){aliasString}";
        }

        public static string Sum(string column, string? alias = null)
        {
            var aliasString = alias != null ? $" AS {alias}" : $"Sum{column}";
            return $"SUM({column}){aliasString}";
        }

        public static string Avg<T>(Expression<Func<T, object>> column, string? alias = null) where T : class
        {
            var columnName = ExpressionHelper.GetColumnNameFromEntity(column);
            var aliasString = alias != null ? $" AS {alias}" : $"Avg{columnName}";
            return $"AVG({columnName}){aliasString}";
        }

        public static string Avg(string column, string? alias = null)
        {
            var aliasString = alias != null ? $" AS {alias}" : $"Avg{column}";
            return $"AVG({column}){aliasString}";
        }

        public static string Max<T>(Expression<Func<T, object>> column, string? alias = null) where T : class
        {
            var columnName = ExpressionHelper.GetColumnNameFromEntity(column);
            var aliasString = alias != null ? $" AS {alias}" : $"Max{columnName}";
            return $"MAX({columnName}){aliasString}";
        }

        public static string Max(string column, string? alias = null)
        {
            var aliasString = alias != null ? $" AS {alias}" : $"Max{column}";
            return $"MAX({column}){aliasString}";
        }

        public static string Min<T>(Expression<Func<T, object>> column, string? alias = null) where T : class
        {
            var columnName = ExpressionHelper.GetColumnNameFromEntity(column);
            var aliasString = alias != null ? $" AS {alias}" : $"Min{columnName}";
            return $"MIN({columnName}){aliasString}";
        }

        public static string Min(string column, string? alias = null)
        {
            var aliasString = alias != null ? $" AS {alias}" : $"Min{column}";
            return $"MIN({column}){aliasString}";
        }
    }
}
