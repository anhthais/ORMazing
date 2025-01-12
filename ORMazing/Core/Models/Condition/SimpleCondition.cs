using ORMazing.Core.Models.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ORMazing.Core.Models.Condition
{
    public class SimpleCondition<T> : Condition<T> where T : class
    {
        private readonly Expression<Func<T, object>> _column;
        private readonly string _operator;
        private readonly object _value;

        public SimpleCondition(Expression<Func<T, object>> column, string @operator, object value)
        {
            _column = column;
            _operator = @operator;
            _value = value;
        }

        public override string ToSql()
        {
            var columnName = ExpressionHelper.GetColumnNameFromEntity<T>(_column);
            var formattedValue = ExpressionHelper.FormatValue(_value);
            return $"{columnName} {_operator} {formattedValue}";
        }
    }

}
