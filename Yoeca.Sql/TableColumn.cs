using System;
using System.Linq;
using System.Reflection;
using Yoeca.Sql.Converters;

namespace Yoeca.Sql
{
    public record  TableColumn (DataType DataType, int Size, string Name, bool NotNull, bool PrimaryKey,
                            bool AutoIncrement, int Decimals = 8, int DecimalFraction = 2)
    {
       

        
        public static TableColumn FixedText(string name, int size, bool notNull, bool primaryKey)
        {
            return new TableColumn(DataType.FixedText, size, name, notNull, primaryKey, false);
        }

        
        public static TableColumn VariableText(
             string name,
            bool notNull,
            bool primaryKey,
            int maximumSize = 0
        )
        {
            return new TableColumn(DataType.VariableText, maximumSize, name, notNull, primaryKey, false);
        }

        
        public static TableColumn Integer( string name, bool primaryKey, bool autoIncrement = false)
        {
            return new TableColumn(DataType.Integer, 0, name, false, primaryKey, autoIncrement);
        }

        
        public static TableColumn Long( string name, bool primaryKey, bool autoIncrement = false)
        {
            return new TableColumn(DataType.Long, 0, name, false, primaryKey, autoIncrement);
        }

        
        public static TableColumn UnsignedLong( string name, bool primaryKey, bool autoIncrement = false)
        {
            return new TableColumn(DataType.UnsignedLong, 0, name, false, primaryKey, autoIncrement);
        }

        
        public static TableColumn Blob( string name, bool notNull, bool primaryKey)
        {
            return new TableColumn(DataType.Binary, 0, name, notNull, primaryKey, false);
        }

        
        public static TableColumn Double( string name, bool hasSqlPrimaryKey)
        {
            return new TableColumn(DataType.Double, 0, name, false, hasSqlPrimaryKey, false);
        }

        public static TableColumn Decimal(string name, bool hasSqlPrimaryKey, int decimals = 8, int decimalFraction = 2)
        {
            return new TableColumn(DataType.Decimal, 0, name, false, hasSqlPrimaryKey, false, decimals, decimalFraction);
        }

        public static TableColumn Time(string name, bool notNull, bool primaryKey)
        {
            return new TableColumn(DataType.Time, 0, name, notNull, primaryKey, false);
        }

        public static TableColumn Date(string name, bool notNull, bool primaryKey)
        {
            return new TableColumn(DataType.Date, 0, name, notNull, primaryKey, false);
        }

        public string Format(SqlFormat format)
        {
            string result = PreFormat(format);

            if (NotNull)
            {
                result += " NOT NULL";
            }

            if (AutoIncrement)
            {
                result += " AUTO_INCREMENT";
            }

            return result;
        }

        
        private string PreFormat(SqlFormat format)
        {
            string identifier = SqlIdentifier.Quote(Name, format);

            switch (DataType)
            {
                case DataType.FixedText:
                    return $"{identifier} CHAR({Size})";
                case DataType.VariableText:
                    if (Size > 0 && Size <= 255)
                    {
                        return $"{identifier} VARCHAR({Size})";
                    }

                    return $"{identifier} TEXT";
                case DataType.Integer:
                    return $"{identifier} INT SIGNED";
                case DataType.UnsignedInteger:
                    return $"{identifier} INT UNSIGNED";
                case DataType.Double:
                    return $"{identifier} DOUBLE";
                case DataType.Long:
                    return $"{identifier} BIGINT SIGNED";
                case DataType.UnsignedLong:
                    return $"{identifier} BIGINT UNSIGNED";
                case DataType.Binary:
                    if (Size > 16777215)
                    {
                        return $"{identifier} LONGBLOB";
                    }

                    if (Size > 65535)
                    {
                        return $"{identifier} MEDIUMBLOB";
                    }

                    return $"{identifier} BLOB";
                case DataType.Decimal:
                    return $"{identifier} DECIMAL({Decimals},{DecimalFraction})";
                case DataType.Date:
                    return $"{identifier} DATE";
                case DataType.Time:
                    return $"{identifier} TIME";
                default:
                    throw new NotSupportedException("Specified data type is not supported: " + DataType);
            }
        }

        public static int GetFixedSize( PropertyInfo property, int defaultSize = -1)
        {
            var attribute = property.GetCustomAttribute<FixedSizeAttribute>();

            return attribute?.Size ?? -1;
        }

        public static int GetMaximumSize( PropertyInfo property, int defaultSize = -1)
        {
            var attribute = property.GetCustomAttribute<MaximumSizeAttribute>();

            return attribute?.Size ?? defaultSize;
        }

        public static bool HasSqlNotNull( PropertyInfo property)
        {
            return Attribute.GetCustomAttributes(property, typeof(SqlNotNullAttribute)).Any();
        }

        public static bool HasSqlPrimaryKey( PropertyInfo property)
        {
            return Attribute.GetCustomAttributes(property, typeof(SqlPrimaryKeyAttribute)).Any();
        }

        public static bool HasAutoIncrement( PropertyInfo property)
        {
            return Attribute.GetCustomAttributes(property, typeof(AutoIncrementAttribute)).Any();
        }

        
        public static TableColumn Create( PropertyInfo property)
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
