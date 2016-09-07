using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal sealed class EnumColumnConverter : IColumnConverter
    {
        public ColumnRetriever TryGet(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.PropertyType.IsEnum)
            {
                return null;
            }

            var converter = new EnumIntegerCoverter(propertyInfo.PropertyType);
            var column = TableColumn.Integer(propertyInfo.Name, TableColumn.HasSqlPrimaryKey(propertyInfo));

            return new ColumnRetriever(propertyInfo, column, converter, false);
        }
    }
}