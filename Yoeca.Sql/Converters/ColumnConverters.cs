using System.Collections.Generic;
using JetBrains.Annotations;

namespace Yoeca.Sql.Converters
{
    internal static class ColumnConverters
    {
        [NotNull, ItemNotNull]
        public static readonly IEnumerable<IColumnConverter> Default = new IColumnConverter[]
        {
            new TypeConverterColumnConverter(),
            new StringColumnConverter(),
            new GuidColumnConverter(),
            new IntegerColumnConverter(),
            new EnumColumnConverter(),
            new DoubleColumnConverter(),
            new ProtoBufferColumnConverter(),
            new DateTimeColumnConverter()
        };
    }
}