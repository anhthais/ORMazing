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

    public class ColumnExpression : SqlExpression
    {
        private readonly string _columnName;

        public ColumnExpression(string columnName)
        {
            _columnName = columnName;
        }

        public override string ToSql() => _columnName;
    }

    public class SqlFunctionExpression : SqlExpression
    {
        private readonly string _function;
        private readonly SqlExpression _argument;

        public SqlFunctionExpression(string function, SqlExpression argument)
        {
            _function = function;
            _argument = argument;
        }

        public override string ToSql() => $"{_function}({_argument.ToSql()})";
    }

    public class ExpressionBuilder<T>
    {
        public SqlExpression Column(Expression<Func<T, object>> expression)
        {
            var columnName = new SqlQueryBuilder<T>().GetColumnNameFromExpression(expression);
            return new ColumnExpression(columnName);
        }

        public SqlExpression Count(Expression<Func<T, object>> expression)
        {
            var column = Column(expression);
            return new SqlFunctionExpression("COUNT", column);
        }

        public SqlExpression Sum(Expression<Func<T, object>> expression)
        {
            var column = Column(expression);
            return new SqlFunctionExpression("SUM", column);
        }
    }
}
