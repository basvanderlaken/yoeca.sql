using System;
using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal sealed class GuidColumnConverter : IColumnConverter
    {
        public ColumnRetriever TryGet(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType != typeof(Guid))
            {
                return null;
            }

            TableColumn column = TableColumn.FixedText(propertyInfo.Name,
                32,
                true,
                TableColumn.HasSqlPrimaryKey(propertyInfo));

            return new ColumnRetriever(propertyInfo, column, new GuidStringCoverter(), true);
        }
    }
}