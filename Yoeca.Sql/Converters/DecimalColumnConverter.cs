using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal sealed class DecimalColumnConverter : IColumnConverter
    {
        public ColumnRetriever? TryGet(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType != typeof(Decimal))
            {
                return null;
            }

            TableColumn column = TableColumn.Decimal(propertyInfo.Name,
                                                    TableColumn.HasSqlPrimaryKey(propertyInfo));

            return new ColumnRetriever(propertyInfo, column, new DecimalStringCoverter(), false);
        }
    }
}