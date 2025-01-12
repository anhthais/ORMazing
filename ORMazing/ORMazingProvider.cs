using ORMazing.Config;
using ORMazing.DataAccess.Executors;
using ORMazing.DataAccess.Factories;
using ORMazing.DataAccess.QueryBuilders;
using ORMazing.DataAccess.Repositories;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORMazing
{
    public class ORMazingProvider
    {
        private static ORMazingProvider _db;
        private string _connectionString;
        private DatabaseType _dbType;

        public static ORMazingProvider DB
        {
            get
            {
                if (_db == null)
                {
                    throw new InvalidOperationException("ORMazing is not configured. Call Configure() first.");
                }
                return _db;
            }
        }

        public IDatabaseFactory DatabaseFactory { get; private set; }

        private ORMazingProvider(string connectionString, DatabaseType dbType)
        {
            _connectionString = connectionString;
            _dbType = dbType;
            SetConnectionFactory();
        }

        public static void Configure(string connectionString, DatabaseType dbType)
        {
            if (_db != null)
            {
                throw new InvalidOperationException("ORMazing is already configured.");
            }

            _db = new ORMazingProvider(connectionString, dbType);
        }

        private void SetConnectionFactory()
        {
            switch (_dbType)
            {
                case DatabaseType.SQLServer:
                    this.DatabaseFactory = new SqlDatabaseFactory(_connectionString);
                    break;
                case DatabaseType.MySQL:
                    throw new NotImplementedException("MySQL not implemented yet");
                default:
                    throw new InvalidOperationException("Unsupported Database Type");
            }
        }

        public RepositoryBase<T> GetRepository<T>() where T : class, new()
        {
            var connection = DatabaseFactory.CreateConnection();
            var queryExecutor = DatabaseFactory.CreateQueryExecutor(connection);
            var queryBuilder = DatabaseFactory.CreateQueryBuilder<T>();
            return new RepositoryBase<T>(queryExecutor, queryBuilder);
        }

        public bool TestConnection()
        {
            try
            {
                using (var connection = DatabaseFactory.CreateConnection())
                {
                    connection.Open();
                    return connection.State == System.Data.ConnectionState.Open;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }
    }
}
