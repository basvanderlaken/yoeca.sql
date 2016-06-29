using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using ProtoBuf;
using Yoeca.Sql.Converters;

namespace Yoeca.Sql
{
    internal sealed class TableDefinition
    {
        public readonly ImmutableList<ColumnRetriever> Columns;
        public readonly Type DataType;
        public readonly string Name;

        public TableDefinition([NotNull] Type dataType)
        {
            DataType = dataType;

            var definition = dataType.GetCustomAttributes(false).OfType<TableDefinitionAttribute>().Single();

            Name = definition.Name;

            var properties = new List<ColumnRetriever>();

            foreach (var property in dataType.GetProperties())
            {
                ColumnRetriever column = ColumnRetriever.TryCreate(property);

                if (column != null)
                {
                    properties.Add(column);
                }
            }

            Columns = properties.ToImmutableList();
        }


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

            public ColumnRetriever([NotNull] string name, [NotNull] PropertyInfo property)
            {
                Name = name;
                Property = property;
                TableColumn = TableColumn.Create(property);

                Convert = GetConverter(property);

                RequiresEscaping = !(Convert is BinaryConverter);
            }

            public bool RequiresEscaping { get; }

            [NotNull]
            private TypeConverter GetConverter([NotNull] PropertyInfo property)
            {
                var attribute = property.GetCustomAttribute<TypeConverterAttribute>() ??
                                property.PropertyType.GetCustomAttribute<TypeConverterAttribute>();

                if (attribute != null)
                {
                    Type type = Type.GetType(attribute.ConverterTypeName);

                    var converter = (TypeConverter) Activator.CreateInstance(type);

                    if (converter.CanConvertTo(typeof(string)))
                    {
                        return converter;
                    }

                    if (converter.CanConvertTo(typeof(byte[])))
                    {
                        return new BinaryConverter(converter);
                    }

                    throw new InvalidOperationException("The converter is not supported.");
                }

                if (property.PropertyType == typeof(string))
                {
                    return new StringConverter();
                }

                if (property.PropertyType == typeof(Guid))
                {
                    return new GuidStringCoverter();
                }

                if (property.PropertyType == typeof(int))
                {
                    return new IntStringCoverter();
                }

                if (property.PropertyType == typeof(byte[]))
                {
                    return new FailingConverter();
                }

                var protoContract = property.PropertyType.GetCustomAttribute<ProtoContractAttribute>();

                if (protoContract != null)
                {
                    var converterType = typeof(ProtoBufferConverter<>).MakeGenericType(property.PropertyType);
                    var converter = (TypeConverter) Activator.CreateInstance(converterType);

                    return new BinaryConverter(converter);
                }

                throw new NotSupportedException("Unsupported type " + property.PropertyType);
            }

            public void Set(object value, object target)
            {
                object propertyValue;

                if (value == null || value.GetType() == Property.PropertyType)
                {
                    propertyValue = value;
                }
                else
                {
                    if (Convert.CanConvertFrom(value.GetType()))
                    {
                        propertyValue = Convert.ConvertFrom(value);
                    }
                    else
                    {
                        propertyValue = Convert.ConvertFromString(value.ToString());
                    }
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
                return new ColumnRetriever(property.Name, property);
            }

            [CanBeNull]
            public string Get([NotNull] object record)
            {
                var propertyValue = Property.GetMethod.Invoke(record, new object[0]);

                return Convert.ConvertToString(propertyValue);
            }
        }
    }
}