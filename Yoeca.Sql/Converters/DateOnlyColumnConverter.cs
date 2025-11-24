using System;
using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal sealed class DateOnlyColumnConverter : IColumnConverter
    {
        public ColumnRetriever? TryGet(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType != typeof(DateOnly))
            {
                return null;
            }

            var converter = new DateOnlyStringConverter();
            var column = TableColumn.Date(propertyInfo.Name,
                                          TableColumn.HasSqlNotNull(propertyInfo),
                                          TableColumn.HasSqlPrimaryKey(propertyInfo));

            return new ColumnRetriever(propertyInfo, column, converter, true);
        }
    }
}
