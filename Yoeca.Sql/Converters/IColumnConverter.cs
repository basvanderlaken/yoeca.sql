using System.Reflection;
using JetBrains.Annotations;

namespace Yoeca.Sql.Converters
{
    internal interface IColumnConverter
    {
        [CanBeNull]
        ColumnRetriever TryGet([NotNull] PropertyInfo propertyInfo);
    }
}