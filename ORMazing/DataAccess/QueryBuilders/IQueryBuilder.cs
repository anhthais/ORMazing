using System.Linq.Expressions;
using ORMazing.Core.Models.Condition;
using ORMazing.Core.Common;

namespace ORMazing.DataAccess.QueryBuilders
{
    public interface IQueryBuilder<T> where T : class, new()
    {
        // SELECT
        IQueryBuilder<T> Select(string[] columns);
        IQueryBuilder<T> Select(params Expression<Func<T, object>>[] selectors);
        IQueryBuilder<T> Select<TResult>(Expression<Func<T, TResult>> selector);

        // WHERE
        IQueryBuilder<T> Where(Condition<T> condition);

        // INSERT, UPDATE, DELETE
        IQueryBuilder<T> Insert(Dictionary<string, object> values);
        IQueryBuilder<T> Update(Dictionary<string, object> values);
        IQueryBuilder<T> Delete(Dictionary<string, object> values);

        // GROUP BY
        IQueryBuilder<T> GroupBy(string[] columns);
        IQueryBuilder<T> GroupBy(params Expression<Func<T, object>>[] selectors);

        // HAVING
        IQueryBuilder<T> Having(Condition<T> condition);

        // ORDER BY
        IQueryBuilder<T> OrderBy(params (string Column, OrderType Order)[] columns);
        IQueryBuilder<T> OrderBy(params (Expression<Func<T, object>> selector, OrderType orderType)[] selectors);

        void Reset();
        string Build();

        Dictionary<string, object> GetParameters();
    }
}
