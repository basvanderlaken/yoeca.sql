using System;
using System.ComponentModel;
using System.Reflection;
using ProtoBuf;

namespace Yoeca.Sql.Converters
{
    internal sealed class ProtoBufferColumnConverter : IColumnConverter
    {
        public ColumnRetriever? TryGet(PropertyInfo propertyInfo)
        {
            var protoContract = propertyInfo.PropertyType.GetCustomAttribute<ProtoContractAttribute>();

            if (protoContract != null)
            {
                var converterType = typeof(ProtoBufferConverter<>).MakeGenericType(propertyInfo.PropertyType);
                var converter = Activator.CreateInstance(converterType) as TypeConverter;

                if (converter is null)
                {
                    return null;
                }

                var binaryConverter = new BinaryConverter(converter);

                var column = TableColumn.Blob(propertyInfo.Name,
                                              TableColumn.HasSqlNotNull(propertyInfo),
                                              TableColumn.HasSqlPrimaryKey(propertyInfo));

                return new ColumnRetriever(propertyInfo, column, binaryConverter, false);
            }

            return null;
        }
    }
}