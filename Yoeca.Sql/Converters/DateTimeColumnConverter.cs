using System;
using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal sealed class DateTimeColumnConverter : IColumnConverter
    {
        public ColumnRetriever? TryGet(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType != typeof(DateTime))
            {
                return null;
            }

            var converter = new DateTimeStringCoverter();
            var column = TableColumn.Long(propertyInfo.Name, TableColumn.HasSqlPrimaryKey(propertyInfo));

            return new ColumnRetriever(propertyInfo, column, converter, false);
        }
    }
}