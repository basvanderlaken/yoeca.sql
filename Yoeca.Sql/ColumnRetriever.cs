using System;
using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;
using Yoeca.Sql.Converters;

namespace Yoeca.Sql
{
    internal sealed class ColumnRetriever
    {
        [NotNull]
        public readonly TypeConverter Convert;

        [NotNull]
        public readonly string Name;

        [NotNull]
        public readonly PropertyInfo Property;

        [NotNull]
        public readonly TableColumn TableColumn;


        public ColumnRetriever(
            [NotNull] PropertyInfo property,
            [NotNull] TableColumn column,
            [NotNull] TypeConverter converter,
            bool requiresEscaping)
        {
            Name = column.Name;
            Property = property;
            TableColumn = column;
            Convert = converter;
            RequiresEscaping = requiresEscaping;
        }

        public bool RequiresEscaping { get; }

        public void Set(object value, object target)
        {
            object propertyValue;

            if (value == null || value.GetType() == Property.PropertyType)
            {
                propertyValue = value;
            }
            else
            {
                propertyValue = Convert.CanConvertFrom(value.GetType())
                    ? Convert.ConvertFrom(value)
                    : Convert.ConvertFromString(value.ToString());
            }
            Property.SetMethod.Invoke(target,
                new[]
                {
                    propertyValue
                });
        }


        [CanBeNull]
        public static ColumnRetriever TryCreate([NotNull] PropertyInfo property)
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

        [CanBeNull]
        public string Get([NotNull] object record)
        {
            var propertyValue = Property.GetMethod.Invoke(record, new object[0]);

            return Convert.ConvertToString(propertyValue);
        }
    }
}