namespace ORMazing.DataAccess.QueryBuilders
{
    public interface IQueryBuilder<T> where T : class, new()
    {
        IQueryBuilder<T> Select(string[] columns);
        IQueryBuilder<T> Where(string condition);
        IQueryBuilder<T> Insert(Dictionary<string, object> values);
        IQueryBuilder<T> Update(Dictionary<string, object> values);
        IQueryBuilder<T> Delete(Dictionary<string, object> values);
        IQueryBuilder<T> GroupBy(string columns);
        IQueryBuilder<T> Having(string condition);
        IQueryBuilder<T> OrderBy(string columns);

        Dictionary<string, object> GetParameters();
        string Build();
    }
}
