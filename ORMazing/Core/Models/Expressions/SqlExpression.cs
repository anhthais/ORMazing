using ORMazing.DataAccess.QueryBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ORMazing.Core.Models.Expressions
{
    public class SqlExpression
    {
        public virtual string ToSql() => "";
    }
}
