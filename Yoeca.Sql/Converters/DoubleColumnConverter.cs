using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal sealed class DoubleColumnConverter : IColumnConverter
    {
        public ColumnRetriever? TryGet(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType != typeof(double))
            {
                return null;
            }

            TableColumn column = TableColumn.Double(propertyInfo.Name,
                                                    TableColumn.HasSqlPrimaryKey(propertyInfo));

            return new ColumnRetriever(propertyInfo, column, new DoubleStringCoverter(), false);
        }
    }
}