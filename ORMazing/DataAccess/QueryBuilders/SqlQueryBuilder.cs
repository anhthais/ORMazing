using ORMazing.Core.Attributes;
using System.Diagnostics;
using System.Text;

namespace ORMazing.DataAccess.QueryBuilders
{
    public class SqlQueryBuilder<T> : IQueryBuilder<T> where T : class, new()
    {
        private readonly StringBuilder _query;
        private readonly Dictionary<string, object> _parameters;
        private readonly string _tableName;

        public SqlQueryBuilder()
        {
            _query = new StringBuilder();
            _tableName = AttributeHelper.GetTableName<T>();
            _parameters = new Dictionary<string, object>();
        }

        public IQueryBuilder<T> Select(string columns = "*")
        {
            _query.Clear();
            _query.Append($"SELECT {columns} FROM {_tableName}");
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

            // Thêm phần WHERE từ giá trị đầu tiên
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
