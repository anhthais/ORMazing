using ORMazing.Core.Attributes;
using ORMazing.Core.Mappers;
using ORMazing.DataAccess.Executors;
using ORMazing.DataAccess.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;
using static ORMazing.DataAccess.Repositories.IRepository;

namespace ORMazing.DataAccess.Repositories
{
    public class RepositoryBase<T> : IRepository<T> where T : class, new()
    {
        private readonly IQueryExecutor _queryExecutor;

        public RepositoryBase(IQueryExecutor queryExecutor)
        {
            _queryExecutor = queryExecutor;
        }

        public void Add(T entity)
        {
            var values = EntityMapper.GetEntityValues(entity);
            var builder = new SqlQueryBuilder<T>().Insert(values);
            var sql = builder.Build();
            _queryExecutor.ExecuteNonQuery(sql, builder.GetParameters());
        }

        public List<T> GetAll()
        {
            var builder = new SqlQueryBuilder<T>().Select();
            var sql = builder.Build();
            var result = _queryExecutor.ExecuteQuery<T>(sql, builder.GetParameters());
            return result;
        }
    }
}
