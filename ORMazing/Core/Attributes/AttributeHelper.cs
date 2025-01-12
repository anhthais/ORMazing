using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ORMazing.Core.Attributes
{
    public static class AttributeHelper
    {
        public static string GetTableName<T>() where T : class, new()
        {
            var tableAttribute = typeof(T).GetCustomAttribute<TableAttribute>();
            if (tableAttribute == null)
                throw new Exception($"Table attribute is missing on {typeof(T).Name}");
            return tableAttribute.Name;
        }

        public static string GetColumnName(PropertyInfo property)
        {
            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            return columnAttribute == null
                ? throw new Exception($"Column attribute is missing on property {property.Name}")
                : columnAttribute.Name;
        }

        public static string GetColumnName<T>(string propertyName) where T : class
        {
            var property = typeof(T).GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException($"Property {propertyName} not found in type {typeof(T).Name}");

            return GetColumnName(property);
        }

        public static PropertyInfo[] GetProperties<T>() where T : class, new()
        {
            var properties = typeof(T).GetProperties();
            var result = new List<PropertyInfo>();

            foreach (var property in properties)
            {
                if (Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) != null)
                {
                    result.Add(property);
                }
            }

            return result.ToArray();
        }

    }
}
