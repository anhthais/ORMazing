using Microsoft.Data.SqlClient;
using ORMazing.Core.Attributes;
using ORMazing.DataAccess.Factories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private void AddParameters(SqlCommand command, Dictionary<string, object> parameters)
        {
            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
            }
        }

    }
}
