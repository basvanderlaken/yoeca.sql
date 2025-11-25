namespace Yoeca.Sql.Converters
{
    internal static class ColumnConverters
    {
        public static IEnumerable<IColumnConverter> Default { get; }  = new IColumnConverter[]
        {
            new TypeConverterColumnConverter(),
            new StringColumnConverter(),
            new GuidColumnConverter(),
            new IntegerColumnConverter(),
            new LongColumnConverter(),
            new UnsignedLongColumnConverter(),
            new EnumColumnConverter(),
            new DoubleColumnConverter(),
            new ProtoBufferColumnConverter(),
            new DateTimeColumnConverter(),
            new DateOnlyColumnConverter(),
            new TimeOnlyColumnConverter(),
            new TimeSpanColumnConverter(),
            new DecimalColumnConverter (),
        };
    }
}
