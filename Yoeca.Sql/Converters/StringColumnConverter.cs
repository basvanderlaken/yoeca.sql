using System.ComponentModel;
using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal sealed class StringColumnConverter : IColumnConverter
    {
        public ColumnRetriever? TryGet(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType != typeof(string))
            {
                return null;
            }

            var converter = new StringConverter();

            TableColumn column = GetColumn(propertyInfo);

            return new ColumnRetriever(propertyInfo, column, converter, true);
        }

        private static TableColumn GetColumn( PropertyInfo property)
        {
            int fixedSize = TableColumn.GetFixedSize(property);

            if (fixedSize != -1)
            {
                return TableColumn.FixedText(property.Name,
                                             fixedSize,
                                             TableColumn.HasSqlNotNull(property),
                                             TableColumn.HasSqlPrimaryKey(property));
            }

            int maximumSize = TableColumn.GetMaximumSize(property);

            if (maximumSize != -1)
            {
                return TableColumn.VariableText(property.Name,
                                                TableColumn.HasSqlNotNull(property),
                                                TableColumn.HasSqlPrimaryKey(property),
                                                maximumSize);
            }

            return TableColumn.VariableText(property.Name,
                                            TableColumn.HasSqlNotNull(property),
                                            TableColumn.HasSqlPrimaryKey(property));
        }
    }
}