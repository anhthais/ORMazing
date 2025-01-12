using System.Linq.Expressions;

namespace ORMazing.DataAccess.Repositories
{
    public interface IRepository
    {
        public interface IRepository<TEntity> where TEntity : class, new()
        {
            void Add(TEntity entity);
            List<TEntity> GetAll();
            void Update(TEntity entity);
            void Delete(TEntity entity);
            List<Dictionary<string, object>> GetWithCondition(List<string> selectedColumns, string whereCondition, List<string> groupByColumns, string havingCondition, string orderByColumns);
            List<TResult> Get<TResult>(Expression<Func<TEntity, TResult>> selector, string? whereCondition = null, Dictionary<string, object>? parameters = null) where TResult : class;
        }
    }
}
