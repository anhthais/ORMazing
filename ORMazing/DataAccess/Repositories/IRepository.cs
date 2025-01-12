using ORMazing.Core.Common;
using ORMazing.Core.Models.Condition;
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

            List<TResult> Get<TResult>(
                Expression<Func<TEntity, TResult>> selector, 
                Condition<TEntity>? whereCondition = null,
                string[]? groupByColumns = null,
                Expression<Func<TEntity, object>>[]? groupBySelectors = null,
                Condition<TEntity>? havingCondition = null,
                (string Column, OrderType Order)[]? orderByColumns = null,
                (Expression<Func<TEntity, object>> selector, OrderType orderType)[]? orderBySelectors = null
                ) where TResult : class;

            List<Dictionary<string, object>> Get(
                string[]? columns = null, 
                Expression<Func<TEntity, object>>[]? columnSelectors = null, 
                Condition<TEntity>? whereCondition = null,
                string[]? groupByColumns = null,
                Expression<Func<TEntity, object>>[]? groupBySelectors = null,
                Condition<TEntity>? havingCondition = null,
                (string Column, OrderType Order)[]? orderByColumns = null,
                (Expression<Func<TEntity, object>> selector, OrderType orderType)[]? orderBySelectors = null
                );
        }
    }
}
