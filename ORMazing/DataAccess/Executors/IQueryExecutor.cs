namespace ORMazing.DataAccess.Executors
{
    public interface IQueryExecutor
    {
        int ExecuteNonQuery(string sql, Dictionary<string, object>? parameters);
        List<T> ExecuteQuery<T>(string sql, Dictionary<string, object>? parameters) where T : class, new();
        public List<Dictionary<string, object>> ExecuteQueryToDictionary(string sql, Dictionary<string, object>? parameters);

    }
}
