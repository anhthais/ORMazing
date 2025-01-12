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
            int result;
            try
            {
               result = _sqlQueryExecutor.ExecuteNonQuery(sql, parameters);
            } 
            catch (Exception ex) {
                PrintErrorLog(ex.Message);
                throw;
            }

            return result;
        }

        public List<T> ExecuteQuery<T>(string sql, Dictionary<string, object>? parameters) where T : class, new()
        {
            PrintPrefixLog();
            Console.WriteLine($"Executing SQL: {sql}");
            List<T> result;
            try
            {
                result = _sqlQueryExecutor.ExecuteQuery<T>(sql, parameters);
            }
            catch (Exception ex)
            {
                PrintErrorLog(ex.Message);
                throw;
            }

            return result;
        }

        public List<Dictionary<string, object>> ExecuteQueryToDictionary(string sql, Dictionary<string, object>? parameters)
        {
            PrintPrefixLog();
            Console.WriteLine($"Executing SQL: {sql}");
            
            List<Dictionary<string, object>> result;
            try
            {
                result = _sqlQueryExecutor.ExecuteQueryToDictionary(sql, parameters);
            }
            catch (Exception ex)
            {
                PrintErrorLog(ex.Message);
                throw;
            }

            return result;
        }

        public List<TResult> ExecuteQueryWithExternalMapper<TResult>(string sql, Dictionary<string, object>? parameters, Func<System.Data.IDataReader, TResult> map)
        {
            PrintPrefixLog();
            Console.WriteLine($"Executing SQL: {sql}");

            List<TResult> result;
            try
            {
                result = _sqlQueryExecutor.ExecuteQueryWithExternalMapper(sql, parameters, map);
            }
            catch (Exception ex)
            {
                PrintErrorLog(ex.Message);
                throw;
            }

            return result;
        }

        private void PrintPrefixLog()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[INFO] ");
            Console.ResetColor();
        }

        private void PrintErrorLog(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"[ERROR] {errorMessage}");
            Console.ResetColor();
        }
    }
}
