using System;
using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal sealed class TimeOnlyColumnConverter : IColumnConverter
    {
        public ColumnRetriever? TryGet(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType != typeof(TimeOnly))
            {
                return null;
            }

            var converter = new TimeOnlyStringConverter();
            var column = TableColumn.Time(propertyInfo.Name,
                                          TableColumn.HasSqlNotNull(propertyInfo),
                                          TableColumn.HasSqlPrimaryKey(propertyInfo),
                                          TableColumn.GetTimeFraction(propertyInfo));

            return new ColumnRetriever(propertyInfo, column, converter, true);
        }
    }
}
