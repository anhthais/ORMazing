using ORMazing.Core.Mappers;
using ORMazing.DataAccess.Executors;
using ORMazing.DataAccess.QueryBuilders;
using System.Diagnostics;
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

        public void Update(T entity)
        {
            var values = EntityMapper.GetEntityValues(entity);
            var builder = new SqlQueryBuilder<T>().Update(values);
            var sql = builder.Build();
            _queryExecutor.ExecuteNonQuery(sql, builder.GetParameters());
        }

        public void Delete(T entity)
        {
            var values = EntityMapper.GetEntityValues(entity);

            var builder = new SqlQueryBuilder<T>().Delete(values);
            var sql = builder.Build();
            _queryExecutor.ExecuteNonQuery(sql, builder.GetParameters());
        }

        public List<Dictionary<string, object>> GetWithCondition(
                List<string>? selectedColumns = null,
                string? whereCondition = null,
                List<string>? groupByColumns = null,
                string? havingCondition = null,
                string? orderByColumns = null)
        {
            string columns = selectedColumns != null && selectedColumns.Count > 0
                ? string.Join(", ", selectedColumns)
                : "*";

            var builder = new SqlQueryBuilder<T>().Select(columns);

            if (!string.IsNullOrEmpty(whereCondition))
            {
                builder.Where(whereCondition);
            }

            if (groupByColumns != null && groupByColumns.Count > 0)
            {
                builder.GroupBy(string.Join(", ", groupByColumns)); // Gộp danh sách cột nhóm thành chuỗi
                if (!string.IsNullOrEmpty(havingCondition))
                {
                    builder.Having(havingCondition);
                }
            }

            if (!string.IsNullOrEmpty(orderByColumns))
            {
                builder.OrderBy(orderByColumns);
            }

            var sql = builder.Build();
            Debug.WriteLine(sql);
            return _queryExecutor.ExecuteQueryToDictionary(sql, builder.GetParameters());
        }







    }
}
