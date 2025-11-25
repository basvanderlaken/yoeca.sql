using System;
using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal sealed class TimeSpanColumnConverter : IColumnConverter
    {
        public ColumnRetriever? TryGet(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType != typeof(TimeSpan))
            {
                return null;
            }

            var converter = new TimeSpanStringConverter();
            var column = TableColumn.Time(propertyInfo.Name,
                                          TableColumn.HasSqlNotNull(propertyInfo),
                                          TableColumn.HasSqlPrimaryKey(propertyInfo),
                                          6);

            return new ColumnRetriever(propertyInfo, column, converter, true);
        }
    }
}
