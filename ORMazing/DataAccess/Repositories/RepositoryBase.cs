using ORMazing.Core.Attributes;
using ORMazing.Core.Mappers;
using ORMazing.Core.Models.Condition;
using ORMazing.Core.Models.Expressions;
using ORMazing.DataAccess.Executors;
using ORMazing.DataAccess.QueryBuilders;
using System.Diagnostics;
using System.Linq.Expressions;
using static ORMazing.DataAccess.Repositories.IRepository;

namespace ORMazing.DataAccess.Repositories
{
    public class RepositoryBase<T> : IRepository<T> where T : class, new()
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IQueryBuilder<T> _queryBuilder;

        public RepositoryBase(IQueryExecutor queryExecutor, IQueryBuilder<T> queryBuilder)
        {
            _queryExecutor = queryExecutor;
            _queryBuilder = queryBuilder;
        }

        public void Add(T entity)
        {
            var values = EntityMapper.GetEntityValues(entity);
            var builder = new SqlQueryBuilder<T>().Insert(values);
            var sql = builder.Build();
            _queryExecutor.ExecuteNonQuery(sql, builder.GetParameters());
        }

        public List<T> GetAll()
        {
            var builder = new SqlQueryBuilder<T>().Select("*");
            var sql = builder.Build();
            var result = _queryExecutor.ExecuteQuery<T>(sql, builder.GetParameters());
            return result;
        }

        public void Update(T entity)
        {
            var values = EntityMapper.GetEntityValues(entity);
            var builder = new SqlQueryBuilder<T>().Update(values);
            var sql = builder.Build();
            _queryExecutor.ExecuteNonQuery(sql, builder.GetParameters());
        }

        public void Delete(T entity)
        {
            var values = EntityMapper.GetEntityValues(entity);

            var builder = new SqlQueryBuilder<T>().Delete(values);
            var sql = builder.Build();
            _queryExecutor.ExecuteNonQuery(sql, builder.GetParameters());
        }

        public List<Dictionary<string, object>> GetWithCondition(
                List<string>? selectedColumns = null,
                string? whereCondition = null,
                List<string>? groupByColumns = null,
                string? havingCondition = null,
                string? orderByColumns = null)
        {
            string columns = selectedColumns != null && selectedColumns.Count > 0
                ? string.Join(", ", selectedColumns)
                : "*";

            var builder = new SqlQueryBuilder<T>().Select(columns);

            if (!string.IsNullOrEmpty(whereCondition))
            {
                //builder.Where(whereCondition);
            }

            if (groupByColumns != null && groupByColumns.Count > 0)
            {
                builder.GroupBy(string.Join(", ", groupByColumns));
                if (!string.IsNullOrEmpty(havingCondition))
                {
                    builder.Having(havingCondition);
                }
            }

            if (!string.IsNullOrEmpty(orderByColumns))
            {
                builder.OrderBy(orderByColumns);
            }

            var sql = builder.Build();
            Debug.WriteLine(sql);
            return _queryExecutor.ExecuteQueryToDictionary(sql, builder.GetParameters());
        }


        public List<TResult> Get<TResult>(Expression<Func<T, TResult>> selector, Condition<T>? condition = null) where TResult : class
        {
            _queryBuilder.Reset();
            _queryBuilder.Select(selector);
            

            if (condition != null)
            {
                _queryBuilder.Where(condition);
            }

            var sql = _queryBuilder.Build();
            var columnMappings = ExtractColumnMappings(selector);

            return _queryExecutor.ExecuteQueryWithExternalMapper<TResult>(
                sql,
                _queryBuilder.GetParameters(),
                reader =>
                {
                    var constructor = typeof(TResult).GetConstructors().First();
                    var arguments = columnMappings.Select(mapping =>
                    {
                        var columnIndex = reader.GetOrdinal(mapping.TargetProperty);
                        return reader.IsDBNull(columnIndex) ? null : reader.GetValue(columnIndex);
                    }).ToArray();

                    return (TResult)constructor.Invoke(arguments);
                });
        }

        public List<Dictionary<string, object>> Get (
            string[]? columns = null,
            Expression<Func<T, object>>[]? columnSelectors = null,
            Condition<T>? condition = null) 
        {
            
            if (columnSelectors != null && columnSelectors.Any())
            {
                _queryBuilder.Select(columnSelectors);
            }
            else if (columns != null && columns.Any())
            {
                _queryBuilder.Select(columns);
            }
            else
            {
                _queryBuilder.Select();
            }

            if (condition != null)
            {
                _queryBuilder.Where(condition);
            }

            var sql = _queryBuilder.Build();

            return _queryExecutor.ExecuteQueryToDictionary(sql, _queryBuilder.GetParameters());
        }

        private List<(string SourceColumn, string TargetProperty)> ExtractColumnMappings<TResult>(Expression<Func<T, TResult>> selector)
        {
            var mappings = new List<(string SourceColumn, string TargetProperty)>();

            if (selector.Body is NewExpression newExpr)
            {
                foreach (var argument in newExpr.Arguments.Zip(newExpr.Members, (arg, member) => new { arg, member }))
                {
                    if (argument.arg is MemberExpression memberExpr)
                    {
                        var sourceColumn = AttributeHelper.GetColumnName<T>(memberExpr.Member.Name);
                        mappings.Add((sourceColumn, argument.member.Name));
                    }
                    else if (argument.arg is MethodCallExpression methodCall)
                    {
                        var sourceColumn = methodCall.ToString();
                        mappings.Add((sourceColumn, argument.member.Name));
                    }
                    else if (argument.arg is BinaryExpression binaryExpr)
                    {
                        var sourceColumn = ExpressionHelper.HandleBinaryExpression<T>(binaryExpr);
                        mappings.Add((sourceColumn, argument.member.Name));
                    }
                    else
                    {
                        throw new ArgumentException($"Unsupported selector argument type: {argument.arg.GetType()}");
                    }
                }
            }
            else
            {
                throw new ArgumentException("Selector must be a valid expression creating a new object.");
            }

            return mappings;
        }
    }
}
