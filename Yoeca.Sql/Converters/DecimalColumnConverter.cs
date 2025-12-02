using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal sealed class DecimalColumnConverter : IColumnConverter
    {
        public ColumnRetriever? TryGet(PropertyInfo propertyInfo)
        {
            Type? decimalType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            if (decimalType != typeof(decimal))
            {
                return null;
            }

            TableColumn column = TableColumn.Decimal(propertyInfo.Name,
                                                    TableColumn.HasSqlPrimaryKey(propertyInfo));

            return new ColumnRetriever(propertyInfo, column, new DecimalStringCoverter(), false);
        }
    }
}
