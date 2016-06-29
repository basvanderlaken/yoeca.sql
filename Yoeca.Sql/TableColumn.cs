using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using ProtoBuf;

namespace Yoeca.Sql
{
    public sealed class TableColumn
    {
        public readonly DataType DataType;
        public readonly string Name;
        public readonly bool NotNull;
        public readonly bool PrimaryKey;
        public readonly int Size;

        private TableColumn(DataType dataType, int size, [NotNull] string name, bool notNull, bool primaryKey)
        {
            DataType = dataType;
            Size = size;
            Name = name;
            NotNull = notNull;
            PrimaryKey = primaryKey;
        }

        [NotNull]
        public static TableColumn FixedText([NotNull] string name, int size, bool notNull, bool primaryKey)
        {
            return new TableColumn(DataType.FixedText, size, name, notNull, primaryKey);
        }

        [NotNull]
        public static TableColumn VariableText(
            [NotNull] string name,
            bool notNull,
            bool primaryKey,
            int maximumSize = 0)
        {
            return new TableColumn(DataType.VariableText, maximumSize, name, notNull, primaryKey);
        }

        [NotNull]
        public static TableColumn Integer([NotNull] string name, bool primaryKey)
        {
            return new TableColumn(DataType.Integer, 0, name, false, primaryKey);
        }


        [NotNull]
        public string Format(SqlFormat format)
        {
            string result = PreFormat(format);

            if (NotNull)
            {
                result += " NOT NULL";
            }

            return result;
        }

        [NotNull]
        private string PreFormat(SqlFormat format)
        {
            switch (DataType)
            {
                case DataType.FixedText:
                    return string.Format("{0} CHAR({1})", Name, Size);
                case DataType.VariableText:
                    if (Size > 0 && Size <= 255)
                    {
                        return string.Format("{0} VARCHAR({1})", Name, Size);
                    }
                    return string.Format("{0} TEXT", Name);
                case DataType.Integer:
                    return string.Format("{0} INT", Name);
                case DataType.Binary:
                    if (Size > 16777215)
                    {
                        return string.Format("{0} LONGBLOB", Name);
                    }
                    if (Size > 65535)
                    {
                        return string.Format("{0} MEDIUMBLOB", Name);
                    }

                    return string.Format("{0} BLOB", Name);
                default:
                    throw new NotSupportedException("Specified datatype is not supported: " + DataType);
            }
        }

        private static int GetFixedSize([NotNull] PropertyInfo property, int defaultSize = -1)
        {
            var attribute = property.GetCustomAttribute<FixedSizeAttribute>();

            return attribute?.Size ?? -1;
        }

        private static int GetMaximumSize([NotNull] PropertyInfo property, int defaultSize = -1)
        {
            var attribute = property.GetCustomAttribute<MaximumSizeAttribute>();

            return attribute?.Size ?? defaultSize;
        }

        [NotNull]
        public static TableColumn Create([NotNull] PropertyInfo property)
        {
            bool hasNotNullAttribute = Attribute.GetCustomAttributes(property, typeof(SqlNotNullAttribute)).Any();
            bool hasPrimaryKeyAttribute = Attribute.GetCustomAttributes(property, typeof(SqlPrimaryKeyAttribute)).Any();

            if (property.PropertyType == typeof(string))
            {
                int fixedSize = GetFixedSize(property);

                if (fixedSize != -1)
                {
                    return FixedText(property.Name, fixedSize, hasNotNullAttribute, hasPrimaryKeyAttribute);
                }

                int maximumSize = GetMaximumSize(property);

                if (maximumSize != -1)
                {
                    return VariableText(property.Name, hasNotNullAttribute, hasPrimaryKeyAttribute, maximumSize);
                }

                return VariableText(property.Name, hasNotNullAttribute, hasPrimaryKeyAttribute);
            }

            if (property.PropertyType == typeof(Guid))
            {
                return FixedText(property.Name, 32, true, hasPrimaryKeyAttribute);
            }

            if (property.PropertyType == typeof(Int32))
            {
                return Integer(property.Name, hasPrimaryKeyAttribute);
            }


            var typeConverterAttribute = property.GetCustomAttribute<TypeConverterAttribute>() ??
                                         property.PropertyType.GetCustomAttribute<TypeConverterAttribute>();

            if (typeConverterAttribute != null)
            {
                Type converterType = Type.GetType(typeConverterAttribute.ConverterTypeName);
                if (converterType != null)
                {
                    var converter = Activator.CreateInstance(converterType) as TypeConverter;

                    if (converter != null && converter.CanConvertTo(typeof(byte[])))
                    {
                        return new TableColumn(DataType.Binary,
                            0,
                            property.Name,
                            hasNotNullAttribute,
                            hasPrimaryKeyAttribute);
                    }
                }
            }

            var protoContract = property.PropertyType.GetCustomAttribute<ProtoContractAttribute>();

            if (protoContract != null)
            {
                return new TableColumn(DataType.Binary,
                    GetMaximumSize(property, 0),
                    property.Name,
                    hasNotNullAttribute,
                    hasPrimaryKeyAttribute);
            }

            throw new NotSupportedException("Type '" + property.PropertyType + "' is not a supported column type.");
        }
    }
}