using System;
using System.ComponentModel;
using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal sealed class TypeConverterColumnConverter : IColumnConverter
    {
        public ColumnRetriever? TryGet(PropertyInfo propertyInfo)
        {
            TypeConverter? converter = TryGetConverter(propertyInfo);

            if (converter == null)
            {
                return null;
            }

            bool requiresEscape = true;
            var column = TableColumn.VariableText(propertyInfo.Name,
                                                  TableColumn.HasSqlNotNull(propertyInfo),
                                                  TableColumn.HasSqlPrimaryKey(propertyInfo));

            if (converter is BinaryConverter)
            {
                requiresEscape = false;
                column = TableColumn.Blob(propertyInfo.Name,
                                          TableColumn.HasSqlNotNull(propertyInfo),
                                          TableColumn.HasSqlPrimaryKey(propertyInfo));
            }

            return new ColumnRetriever(propertyInfo, column, converter, requiresEscape);
        }

        private TypeConverter? TryGetConverter(PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttribute<TypeConverterAttribute>() ??
                            propertyInfo.PropertyType.GetCustomAttribute<TypeConverterAttribute>();

            if (attribute != null)
            {
                Type? type = Type.GetType(attribute.ConverterTypeName);

                if (type is null)
                {
                    return null;
                }

                var converter = Activator.CreateInstance(type) as TypeConverter;

                if (converter is null || converter.CanConvertTo(typeof(string)))
                {
                    return converter;
                }

                if (converter.CanConvertTo(typeof(byte[])))
                {
                    return new BinaryConverter(converter);
                }
            }

            return null;
        }
    }
}