using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ORMazing.Core.Models.Condition
{
    public abstract class Condition<T> where T : class
    {
        public abstract string ToSql();

        public static Condition<T> GreaterThan(Expression<Func<T, object>> column, object value) =>
            new SimpleCondition<T>(column, ">", value);

        public static Condition<T> GreaterThanOrEqual(Expression<Func<T, object>> column, object value) =>
            new CompositeCondition<T>("OR", GreaterThan(column, value), Equals(column, value));

        public static Condition<T> LessThan(Expression<Func<T, object>> column, object value) =>
            new SimpleCondition<T>(column, "<", value);

        public static Condition<T> LessThanOrEqual(Expression<Func<T, object>> column, object value) =>
            new CompositeCondition<T>("OR", LessThan(column, value), Equals(column, value));

        public static Condition<T> Equals(Expression<Func<T, object>> column, object value) =>
            new SimpleCondition<T>(column, "=", value);

        public static CompositeCondition<T> And(params Condition<T>[] conditions) =>
            new CompositeCondition<T>("AND", conditions);

        public static CompositeCondition<T> Or(params Condition<T>[] conditions) =>
            new CompositeCondition<T>("OR", conditions);
    }
}
