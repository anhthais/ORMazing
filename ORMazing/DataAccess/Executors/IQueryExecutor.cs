using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMazing.DataAccess.Executors
{
    public interface IQueryExecutor
    {
        int ExecuteNonQuery(string sql, Dictionary<string, object>? parameters);
        List<T> ExecuteQuery<T>(string sql, Dictionary<string, object>? parameters) where T : class, new();
    }
}
