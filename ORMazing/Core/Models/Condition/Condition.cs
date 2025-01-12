using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMazing.Core.Models.Condition
{
    public abstract class Condition
    {
        public abstract string ToSql();
    }
}
