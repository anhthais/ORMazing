using ORMazing.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
