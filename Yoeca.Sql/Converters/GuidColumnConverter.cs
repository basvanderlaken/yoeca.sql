using System;
using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal sealed class GuidColumnConverter : IColumnConverter
    {
        public ColumnRetriever? TryGet(PropertyInfo propertyInfo)
        {
            var propertyType = propertyInfo.PropertyType;

            bool isNullable = propertyType == typeof(Guid?);

            if (propertyType != typeof(Guid) && !isNullable)
            {
                return null;
            }

            bool notNull = !isNullable || TableColumn.HasSqlNotNull(propertyInfo);

            TableColumn column = TableColumn.FixedText(propertyInfo.Name,
                                                       32,
                                                       notNull,
                                                       TableColumn.HasSqlPrimaryKey(propertyInfo));

            return new ColumnRetriever(propertyInfo, column, new GuidStringCoverter(), true);
        }
    }
}
