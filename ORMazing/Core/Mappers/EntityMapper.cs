using ORMazing.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ORMazing.Core.Mappers
{
    public class EntityMapper
    {
        public static Dictionary<string, object> GetEntityValues<T>(T entity) where T : class, new()
        {
            var properties = AttributeHelper.GetProperties<T>();
            var values = new Dictionary<string, object>();

            foreach (var property in properties)
            {
                var columnName = AttributeHelper.GetColumnName(property);
                var value = property.GetValue(entity);
                values[columnName] = value ?? DBNull.Value;
            }

            return values;
        }
    }
}
