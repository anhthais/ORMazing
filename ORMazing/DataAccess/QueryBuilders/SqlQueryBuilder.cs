using ORMazing.Core.Attributes;
using ORMazing.Core.Models.Expressions;
using ORMazing.Core.Models.Condition;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ORMazing.Core.Common;

namespace ORMazing.DataAccess.QueryBuilders
{
    public class SqlQueryBuilder<T> : IQueryBuilder<T> where T : class, new()
    {
        private StringBuilder _query;
        private Dictionary<string, object> _parameters;
        private readonly string _tableName;

        public SqlQueryBuilder()
        {
            _query = new StringBuilder();
            _tableName = AttributeHelper.GetTableName<T>();
            _parameters = new Dictionary<string, object>();
        }

        public IQueryBuilder<T> Select(params string[] columns)
        {
            _query.Clear();

            if (columns.Length == 0)
            {
                _query.Append($"SELECT * FROM {_tableName}");
            }
            else
            {
                _query.Append($"SELECT {string.Join(", ", columns)} FROM {_tableName}");
            }

            return this;
        }
        
        public IQueryBuilder<T> Select(params Expression<Func<T, object>>[] selectors)
        {
            _query.Clear();

            var columnNames = new List<string>();

            if (selectors.Length == 0)
            {
                columnNames.Add("*");
            } 
            else
            {
                foreach(var selector in selectors)
                {
                    var propertyName = ExpressionHelper.GetColumnNameFromEntity<T>(selector);
                    var columnName = AttributeHelper.GetColumnName<T>(propertyName);
                    columnNames.Add(columnName);
                }
            }

            _query.Append($"SELECT {string.Join(", ", columnNames)} FROM {_tableName}");
            return this;
        }
        
        public IQueryBuilder<T> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            this.Reset();

            if (selector.Body is not NewExpression newExpression)
            {
                throw new ArgumentException("Selector must be a valid expression creating a new object.");
            }

            var columnMappings = new List<string>();

            foreach (var argument in newExpression.Arguments.Zip(newExpression.Members, (arg, member) => new { arg, member }))
            {
                string columnName;

                if (argument.arg is MemberExpression memberExpression)
                {
                    var propertyName = memberExpression.Member.Name;
                    columnName = AttributeHelper.GetColumnName<T>(propertyName);
                }
                else if (argument.arg is BinaryExpression binaryExpression)
                {
                    columnName = ExpressionHelper.HandleBinaryExpression<T>(binaryExpression);
                }
                else if (argument.arg is MethodCallExpression methodCall)
                {
                    columnName = ExpressionHelper.ParseAggregateMethodCall<T>(methodCall);
                }
                else
                {
                    throw new ArgumentException($"Unsupported selector argument type: {argument.arg.GetType()}");
                }

                columnMappings.Add($"{columnName} AS {argument.member.Name}");
            }

            _query.Append($"SELECT {string.Join(", ", columnMappings)} FROM {_tableName}");

            return this;
        }
        
        public IQueryBuilder<T> Where(Condition<T> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var conditionSql = condition.ToSql();

            if (_query.ToString().Contains("WHERE"))
            {
                _query.Append($" AND ({conditionSql})");
            } 
            else 
            {
                _query.Append($" WHERE {conditionSql}");
            }

            return this;
        }

        public IQueryBuilder<T> GroupBy(params string[] columns)
        {
            if (columns.Length == 0)
            {
                throw new ArgumentException("At least one column must be provided for GROUP BY clause.", nameof(columns));
            }

            _query.Append($" GROUP BY {string.Join(", ", columns)}");
            return this;
        }

        public IQueryBuilder<T> GroupBy(params Expression<Func<T, object>>[] selectors)
        {
            if (selectors.Length == 0)
            {
                throw new ArgumentException("At least one selector must be provided for GROUP BY clause.", nameof(selectors));
            }

            var columnNames = new List<string>();

            foreach (var selector in selectors)
            {
                var propertyName = ExpressionHelper.GetColumnNameFromEntity<T>(selector);
                var columnName = AttributeHelper.GetColumnName<T>(propertyName);
                columnNames.Add(columnName);
            }

            _query.Append($" GROUP BY {string.Join(", ", columnNames)}");
            return this;
        }

        public IQueryBuilder<T> Having(Condition<T> condition)
        {
            _query.Append($" HAVING {condition.ToSql()}");
            return this;
        }

        public IQueryBuilder<T> OrderBy(params (string Column, OrderType Order)[] columns)
        {
            if (columns.Length > 0)
            {
                var orderByClauses = columns.Select(c => $"{c.Column} {(c.Order == OrderType.Ascending ? "ASC" : "DESC")}");
                _query.Append($" ORDER BY {string.Join(", ", orderByClauses)}");
            }

            return this;
        }

        public IQueryBuilder<T> OrderBy(params (Expression<Func<T, object>> selector, OrderType orderType)[] selectors)
        {
            if (selectors.Length > 0)
            {
                var columnOrders = new List<string>();

                foreach (var (selector, orderType) in selectors)
                {
                    var propertyName = ExpressionHelper.GetColumnNameFromEntity<T>(selector);
                    var columnName = AttributeHelper.GetColumnName<T>(propertyName);

                    var order = orderType == OrderType.Descending ? "DESC" : "ASC";
                    columnOrders.Add($"{columnName} {order}");
                }

                _query.Append($" ORDER BY {string.Join(", ", columnOrders)}");
            }

            return this;
        }

        public IQueryBuilder<T> Insert(Dictionary<string, object> values)
        {
            this.Reset();
            _query.Append($"INSERT INTO {_tableName} (");

            var columns = new List<string>();
            var parameterizedValues = new List<string>();

            foreach (var kvp in values)
            {
                columns.Add(kvp.Key);
                var parameterName = $"@{kvp.Key}";
                parameterizedValues.Add(parameterName);
                _parameters[parameterName] = kvp.Value;
            }

            _query.Append(string.Join(", ", columns));
            _query.Append(") VALUES (");
            _query.Append(string.Join(", ", parameterizedValues));
            _query.Append(")");

            return this;
        }

        public IQueryBuilder<T> Update(Dictionary<string, object> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("Values dictionary cannot be null or empty.", nameof(values));

            this.Reset();
            _query.Append($"UPDATE {_tableName} SET ");

            var updates = new List<string>();
            var conditions = new List<string>();
            bool isFirst = true;

            foreach (var kvp in values)
            {
                var parameterName = $"@{kvp.Key}";

                if (isFirst)
                {
                    // Sử dụng giá trị đầu tiên làm điều kiện WHERE
                    conditions.Add($"{kvp.Key} = {parameterName}");
                    isFirst = false;
                }
                else
                {
                    // Các giá trị còn lại thuộc về phần SET
                    updates.Add($"{kvp.Key} = {parameterName}");
                }

                _parameters[parameterName] = kvp.Value;
            }

            if (updates.Count == 0)
                throw new ArgumentException("At least one value must be updated.", nameof(values));

            _query.Append(string.Join(", ", updates));

            if (conditions.Count > 0)
            {
                _query.Append(" WHERE ");
                _query.Append(string.Join(" AND ", conditions));
            }

            return this;
        }

        public IQueryBuilder<T> Delete(Dictionary<string, object> conditions)
        {
            this.Reset();
            _query.Append($"DELETE FROM {_tableName}");

            if (conditions != null && conditions.Count > 0)
            {
                _query.Append(" WHERE ");
                var conditionList = new List<string>();

                foreach (var kvp in conditions)
                {
                    var parameterName = $"@{kvp.Key}";
                    conditionList.Add($"{kvp.Key} = {parameterName}");
                    _parameters[parameterName] = kvp.Value;
                }

                _query.Append(string.Join(" AND ", conditionList));
            }

            return this;
        }

        public void Reset()
        {
            _query.Clear();
            _parameters.Clear();
        }

        public string Build()
        {
            return _query.ToString();
        }

        public Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>(_parameters);
        }
    }
}
