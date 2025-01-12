using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ORMazing.Core.Models.Condition
{
    public class CompositeCondition<T> : Condition<T> where T : class
    {
        private readonly string _logicalOperator;
        private readonly List<Condition<T>> _conditions;

        public CompositeCondition(string logicalOperator, params Condition<T>[] conditions)
        {
            _logicalOperator = logicalOperator;
            _conditions = conditions.ToList();
        }

        public override string ToSql()
        {
            var sqlParts = _conditions.Select(c => c.ToSql());
            return $"({string.Join($" {_logicalOperator} ", sqlParts)})";
        }

        public void AddCondition(Condition<T> condition)
        {
            _conditions.Add(condition);
        }

        public void RemoveCondition(Condition<T> condition)
        {
            _conditions.Remove(condition);
        }

        public void ClearConditions()
        {
            _conditions.Clear();
        }

        public Condition<T>[] GetConditions()
        {
            return _conditions.ToArray();
        }
    }

}
