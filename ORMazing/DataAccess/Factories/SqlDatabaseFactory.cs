using Microsoft.Data.SqlClient;
using ORMazing.DataAccess.Executors;
using ORMazing.DataAccess.QueryBuilders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMazing.DataAccess.Factories
{
    public class SqlDatabaseFactory : IDatabaseFactory
    {
        private readonly string _connectionString;

        public SqlDatabaseFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public IQueryExecutor CreateQueryExecutor(IDbConnection connection, bool logging = false)
        {
            var sqlQueryExecutor = new SqlQueryExecutor(connection);

            if (logging)
            {
                return new LoggingSqlQueryExecutor(sqlQueryExecutor);
            } 
            else
            {
                return sqlQueryExecutor;
            }
        }

        public IQueryBuilder<T> CreateQueryBuilder<T>() where T : class, new()
        {
            return new SqlQueryBuilder<T>();
        }
    }

}
