using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal sealed class BoolColumnConverter : IColumnConverter
    {
        public ColumnRetriever? TryGet(PropertyInfo propertyInfo)
        {
            Type? boolType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            if (boolType != typeof(bool))
            {
                return null;
            }

            var column = TableColumn.Integer(propertyInfo.Name, TableColumn.HasSqlPrimaryKey(propertyInfo));

            if (TableColumn.HasSqlNotNull(propertyInfo))
            {
                column = column with { NotNull = true };
            }

            return new ColumnRetriever(propertyInfo, column, new BoolStringConverter(), false);
        }
    }
}
