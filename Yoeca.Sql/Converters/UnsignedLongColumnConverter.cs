using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal sealed class UnsignedLongColumnConverter : IColumnConverter
    {
        public ColumnRetriever TryGet(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType != typeof(ulong))
            {
                return null;
            }

            var converter = new UnsignedLongStringCoverter();
            var column = TableColumn.UnsignedLong(propertyInfo.Name,
                                                  TableColumn.HasSqlPrimaryKey(propertyInfo),
                                                  TableColumn.HasAutoIncrement(propertyInfo));

            return new ColumnRetriever(propertyInfo, column, converter, false);
        }
    }
}