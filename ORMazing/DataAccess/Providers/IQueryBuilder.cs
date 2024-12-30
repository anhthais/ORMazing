using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ORMazing.DataAccess.Providers
{
    public interface IQueryBuilder<T> where T : class, new()
    {
        IQueryBuilder<T> Select(string columns = "*");
        // TODO: condition will be Condition object in the future
        IQueryBuilder<T> Where(string condition);
        IQueryBuilder<T> Insert(Dictionary<string, object> values);
        Dictionary<string, object> GetParameters();
        string Build();
    }
}
