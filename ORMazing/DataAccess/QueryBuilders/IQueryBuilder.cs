using System.Linq.Expressions;

namespace ORMazing.DataAccess.QueryBuilders
{
    public interface IQueryBuilder<T> where T : class, new()
    {
        IQueryBuilder<T> Select(string[] columns);
        IQueryBuilder<T> Select(params Expression<Func<T, object>>[] selectors);
        IQueryBuilder<T> Select<TResult>(Expression<Func<T, TResult>> selector);
        IQueryBuilder<T> Where(string condition);
        IQueryBuilder<T> Insert(Dictionary<string, object> values);
        IQueryBuilder<T> Update(Dictionary<string, object> values);
        IQueryBuilder<T> Delete(Dictionary<string, object> values);
        IQueryBuilder<T> GroupBy(string columns);
        IQueryBuilder<T> Having(string condition);
        IQueryBuilder<T> OrderBy(string columns);

        void Reset();
        string Build();

        Dictionary<string, object> GetParameters();
    }
}
