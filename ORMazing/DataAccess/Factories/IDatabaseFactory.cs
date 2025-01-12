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
    public interface IDatabaseFactory
    {
        IDbConnection CreateConnection();
        IQueryExecutor CreateQueryExecutor(IDbConnection connection);
        IQueryBuilder<T> CreateQueryBuilder<T>() where T : class, new();
    }
}
