using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMazing.DataAccess.Factories
{
    public interface IDatabaseConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
