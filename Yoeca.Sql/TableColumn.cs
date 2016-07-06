using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Yoeca.Sql.Converters;

namespace Yoeca.Sql
{
    public sealed class TableColumn
    {
        public readonly DataType DataType;

        [NotNull]
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
        public static TableColumn Long([NotNull] string name, bool primaryKey)
        {
            return new TableColumn(DataType.Long, 0, name, false, primaryKey);
        }

        [NotNull]
        public static TableColumn Blob([NotNull] string name, bool notNull, bool primaryKey)
        {
            return new TableColumn(DataType.Binary, 0, name, notNull, primaryKey);
        }

        [NotNull]
        public static TableColumn Double([NotNull] string name, bool hasSqlPrimaryKey)
        {
            return new TableColumn(DataType.Double, 0, name, false, hasSqlPrimaryKey);
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
                case DataType.Double:
                    return string.Format("{0} DOUBLE", Name);
                case DataType.Long:
                    return string.Format("{0} BIGINT", Name);
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

        public static int GetFixedSize([NotNull] PropertyInfo property, int defaultSize = -1)
        {
            var attribute = property.GetCustomAttribute<FixedSizeAttribute>();

            return attribute?.Size ?? -1;
        }

        public static int GetMaximumSize([NotNull] PropertyInfo property, int defaultSize = -1)
        {
            var attribute = property.GetCustomAttribute<MaximumSizeAttribute>();

            return attribute?.Size ?? defaultSize;
        }

        public static bool HasSqlNotNull([NotNull] PropertyInfo property)
        {
            return Attribute.GetCustomAttributes(property, typeof(SqlNotNullAttribute)).Any();
        }

        public static bool HasSqlPrimaryKey([NotNull] PropertyInfo property)
        {
            return Attribute.GetCustomAttributes(property, typeof(SqlPrimaryKeyAttribute)).Any();
        }

        [NotNull]
        public static TableColumn Create([NotNull] PropertyInfo property)
        {
            foreach (var columnConverter in ColumnConverters.Default)
            {
                var retriever = columnConverter.TryGet(property);

                if (retriever != null)
                {
                    return retriever.TableColumn;
                }
            }

            throw new NotSupportedException("Type '" + property.PropertyType + "' is not a supported column type.");
        }
    }
}