using ORMazing.Config;
using ORMazing.DataAccess.Executors;
using ORMazing.DataAccess.Factories;
using ORMazing.DataAccess.Repositories;

namespace ORMazing
{

    public class ORMazingProvider
    {
        private static ORMazingProvider _db;
        private string connectionString;
        private DatabaseType dbType;


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

        public IDatabaseConnectionFactory ConnectionFactory { get; private set; }

        private ORMazingProvider(string connectionString, DatabaseType dbType)
        {
            this.connectionString = connectionString;
            this.dbType = dbType;
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
            switch (dbType)
            {
                case DatabaseType.SQLServer:
                    this.ConnectionFactory = new SqlConnectionFactory(connectionString);
                    break;
                case DatabaseType.MySQL:
                    throw new NotImplementedException("MySQL not implemented yet");
                default:
                    throw new InvalidOperationException("Unsupported Database Type");
            }
        }

        public RepositoryBase<T> GetRepository<T>() where T : class, new()
        {
            return new RepositoryBase<T>(new SqlQueryExecutor(ConnectionFactory));
        }

        public bool TestConnection()
        {
            try
            {
                using (var connection = ConnectionFactory.CreateConnection())
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
