using ORMazing.Core.Attributes;
using ORMazing.Core.Models.Expressions;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

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
                    var propertyName = GetColumnNameFromExpression(selector);
                    var columnName = AttributeHelper.GetColumnName<T>(propertyName);
                    columnNames.Add(columnName);
                }
            }

            _query.Append($"SELECT {string.Join(", ", columnNames)} FROM {_tableName}");
            return this;
        }
        public IQueryBuilder<T> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            _query.Clear();

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
                    columnName = methodCall.ToString();
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
        public IQueryBuilder<T> Where(string condition)
        {
            _query.Append($" WHERE {condition}");
            return this;
        }
        public IQueryBuilder<T> GroupBy(string columns)
        {
            _query.Append($" GROUP BY {columns}");
            return this;
        }
        public IQueryBuilder<T> Having(string condition)
        {
            _query.Append($" HAVING {condition}");
            return this;
        }

        public IQueryBuilder<T> OrderBy(string columns)
        {
            _query.Append($" ORDER BY {columns}");
            return this;
        }
        public IQueryBuilder<T> Insert(Dictionary<string, object> values)
        {
            _query.Clear();
            _query.Append($"INSERT INTO {_tableName} (");

            var columns = new List<string>();
            var parameterizedValues = new List<string>();

            _parameters.Clear();
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
            Debug.WriteLine(_query.ToString());
            return this;
        }

        public IQueryBuilder<T> Update(Dictionary<string, object> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("Values dictionary cannot be null or empty.", nameof(values));

            _query.Clear();
            _query.Append($"UPDATE {_tableName} SET ");

            var updates = new List<string>();
            var conditions = new List<string>();

            bool isFirst = true;
            _parameters.Clear();

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
            _query.Clear();
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

        private string GetColumnNameFromExpression(Expression<Func<T, object>> expression)
        {
            if (expression.Body is MemberExpression member)
            {
                return member.Member.Name;
            }
            if (expression.Body is UnaryExpression unary && unary.Operand is MemberExpression memberOperand)
            {
                return memberOperand.Member.Name;
            }
            throw new InvalidOperationException("Invalid column expression.");
        }
    }
}
