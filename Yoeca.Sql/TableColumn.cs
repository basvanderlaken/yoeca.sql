using System;
using System.Linq;
using System.Reflection;
using Yoeca.Sql.Converters;

namespace Yoeca.Sql
{
    public sealed class TableColumn
    {
        public readonly DataType DataType;

        
        public readonly string Name;

        public readonly bool NotNull;
        public readonly bool PrimaryKey;
        public readonly int Size;

        /// <summary>
        /// Flag indicating the column will be auto-incremented by the database.
        /// </summary>
        /// <seealso cref="AutoIncrementAttribute"/>
        public readonly bool AutoIncrement;

        private TableColumn(DataType dataType, int size,  string name, bool notNull, bool primaryKey,
                            bool autoIncrement)
        {
            DataType = dataType;
            Size = size;
            Name = name;
            NotNull = notNull;
            PrimaryKey = primaryKey;
            AutoIncrement = autoIncrement;
        }

        
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
            switch (DataType)
            {
                case DataType.FixedText:
                    return $"{Name} CHAR({Size})";
                case DataType.VariableText:
                    if (Size > 0 && Size <= 255)
                    {
                        return $"{Name} VARCHAR({Size})";
                    }

                    return $"{Name} TEXT";
                case DataType.Integer:
                    return $"{Name} INT SIGNED";
                case DataType.UnsignedInteger:
                    return $"{Name} INT UNSIGNED";
                case DataType.Double:
                    return $"{Name} DOUBLE";
                case DataType.Long:
                    return $"{Name} BIGINT SIGNED";
                case DataType.UnsignedLong:
                    return $"{Name} BIGINT UNSIGNED";
                case DataType.Binary:
                    if (Size > 16777215)
                    {
                        return $"{Name} LONGBLOB";
                    }

                    if (Size > 65535)
                    {
                        return $"{Name} MEDIUMBLOB";
                    }

                    return $"{Name} BLOB";
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