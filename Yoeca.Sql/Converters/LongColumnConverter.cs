using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal sealed class LongColumnConverter : IColumnConverter
    {
        public ColumnRetriever? TryGet(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType != typeof(long))
            {
                return null;
            }

            var converter = new LongStringCoverter();
            var column = TableColumn.Long(propertyInfo.Name,
                                          TableColumn.HasSqlPrimaryKey(propertyInfo),
                                          TableColumn.HasAutoIncrement(propertyInfo));

            return new ColumnRetriever(propertyInfo, column, converter, false);
        }
    }
}