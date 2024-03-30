using System.ComponentModel;
using System.Reflection;
using Yoeca.Sql.Converters;

namespace Yoeca.Sql
{
    internal sealed class ColumnRetriever
    {
        public TypeConverter Convert { get; }

        public string Name { get; }

        public PropertyInfo Property { get; }

        public TableColumn TableColumn { get; }


        public ColumnRetriever(
            PropertyInfo property,
            TableColumn column,
            TypeConverter converter,
            bool requiresEscaping)
        {
            Name = column.Name;
            Property = property;
            TableColumn = column;
            Convert = converter;
            RequiresEscaping = requiresEscaping;
        }

        public bool RequiresEscaping
        {
            get;
        }

        public void Set(object? value, object target)
        {
            object? propertyValue = GetValue(value);

            Property.SetMethod?.Invoke(target,
                                      new[]
                                      {
                                          propertyValue
                                      });
        }

        public object? GetValue(object? value)
        {
            object? propertyValue;

            if (value == null || value.GetType() == Property.PropertyType)
            {
                propertyValue = value;
            }
            else
            {
                propertyValue = Convert.CanConvertFrom(value.GetType())
                    ? Convert.ConvertFrom(value)
                    : Convert.ConvertFromString(value?.ToString() ?? string.Empty);
            }

            return propertyValue;
        }


        public static ColumnRetriever? TryCreate(PropertyInfo property)
        {
            foreach (var columnConverter in ColumnConverters.Default)
            {
                var retriever = columnConverter.TryGet(property);

                if (retriever != null)
                {
                    return retriever;
                }
            }

            throw new NotSupportedException("Unsupported type " + property.PropertyType);
        }

        public string? Get(object record)
        {
            var propertyValue = Property.GetMethod?.Invoke(record, new object[0]);

            return Convert.ConvertToString(propertyValue);
        }
    }
}