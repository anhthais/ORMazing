using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMazing.DataAccess.Executors
{
    public class LoggingSqlQueryExecutor : IQueryExecutor
    {
        private SqlQueryExecutor _sqlQueryExecutor { get; }

        public LoggingSqlQueryExecutor(SqlQueryExecutor sqlQueryExecutor)
        {
            _sqlQueryExecutor = sqlQueryExecutor;
        }

        public int ExecuteNonQuery(string sql, Dictionary<string, object>? parameters)
        {
            PrintPrefixLog();
            Console.WriteLine($"Executing SQL: {sql}");
            return _sqlQueryExecutor.ExecuteNonQuery(sql, parameters);
        }

        public List<T> ExecuteQuery<T>(string sql, Dictionary<string, object>? parameters) where T : class, new()
        {
            PrintPrefixLog();
            Console.WriteLine($"Executing SQL: {sql}");
            return _sqlQueryExecutor.ExecuteQuery<T>(sql, parameters);
        }

        public List<Dictionary<string, object>> ExecuteQueryToDictionary(string sql, Dictionary<string, object>? parameters)
        {
            PrintPrefixLog();
            Console.WriteLine($"Executing SQL: {sql}");
            return _sqlQueryExecutor.ExecuteQueryToDictionary(sql, parameters);
        }

        public List<TResult> ExecuteQueryWithExternalMapper<TResult>(string sql, Dictionary<string, object>? parameters, Func<System.Data.IDataReader, TResult> map)
        {
            PrintPrefixLog();
            Console.WriteLine($"Executing SQL: {sql}");
            return _sqlQueryExecutor.ExecuteQueryWithExternalMapper(sql, parameters, map);
        }

        private void PrintPrefixLog()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[INFO] ");
            Console.ResetColor();
        }
    }
}
