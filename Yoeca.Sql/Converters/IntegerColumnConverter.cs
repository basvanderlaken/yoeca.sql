using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal sealed class IntegerColumnConverter : IColumnConverter
    {
        public ColumnRetriever TryGet(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType != typeof(int))
            {
                return null;
            }

            var converter = new IntegerStringCoverter();
            var column = TableColumn.Integer(propertyInfo.Name,
                                             TableColumn.HasSqlPrimaryKey(propertyInfo),
                                             TableColumn.HasAutoIncrement(propertyInfo));

            return new ColumnRetriever(propertyInfo, column, converter, false);
        }
    }
}