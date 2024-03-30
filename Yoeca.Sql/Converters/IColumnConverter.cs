using System.Reflection;

namespace Yoeca.Sql.Converters
{
    internal interface IColumnConverter
    {
        ColumnRetriever? TryGet(PropertyInfo propertyInfo);
    }
}