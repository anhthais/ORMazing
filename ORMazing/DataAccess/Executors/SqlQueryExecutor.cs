using Microsoft.Data.SqlClient;
using ORMazing.Core.Attributes;
using ORMazing.DataAccess.Factories;
using System.Data;

namespace ORMazing.DataAccess.Executors
{
    public class SqlQueryExecutor : IQueryExecutor
    {
        private readonly IDbConnection _connection;

        public SqlQueryExecutor(IDatabaseConnectionFactory connectionFactory)
        {
            _connection = connectionFactory.CreateConnection();
        }

        public int ExecuteNonQuery(string sql, Dictionary<string, object>? parameters)
        {
            try
            {
                _connection.Open();
                using (var command = new SqlCommand(sql, (SqlConnection)_connection))
                {
                    if (parameters != null)
                    {
                        AddParameters(command, parameters);
                    }

                    return command.ExecuteNonQuery();
                }
            }
            finally
            {
                _connection.Close();
            }
        }

        public List<T> ExecuteQuery<T>(string sql, Dictionary<string, object>? parameters) where T : class, new()
        {
            var results = new List<T>();
            try
            {
                _connection.Open();
                using (var command = new SqlCommand(sql, (SqlConnection)_connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (parameters != null)
                        {
                            AddParameters(command, parameters);
                        }
                        while (reader.Read())
                        {
                            var obj = new T();
                            foreach (var prop in typeof(T).GetProperties())
                            {
                                var columnName = AttributeHelper.GetColumnName(prop);
                                if (!reader.IsDBNull(reader.GetOrdinal(columnName)))
                                {
                                    prop.SetValue(obj, reader.GetValue(reader.GetOrdinal(columnName)));
                                }
                            }
                            results.Add(obj);
                        }
                    }
                }
            }
            finally
            {
                _connection.Close();
            }
            return results;
        }
        public List<Dictionary<string, object>> ExecuteQueryToDictionary(string sql, Dictionary<string, object>? parameters)
        {
            var results = new List<Dictionary<string, object>>();
            try
            {
                _connection.Open();
                using (var command = new SqlCommand(sql, (SqlConnection)_connection))
                {
                    if (parameters != null)
                    {
                        AddParameters(command, parameters);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var columnName = reader.GetName(i);
                                row[columnName] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            results.Add(row);
                        }
                    }
                }
            }
            finally
            {
                _connection.Close();
            }
            return results;
        }

        private void AddParameters(SqlCommand command, Dictionary<string, object> parameters)
        {
            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
            }
        }

    }
}
