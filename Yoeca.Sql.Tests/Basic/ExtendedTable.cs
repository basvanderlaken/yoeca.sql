using System;
using System.ComponentModel;
using System.Globalization;

namespace Yoeca.Sql.NUnit
{
    [SqlTableDefinition("Extended")]
    internal sealed class ExtendedTable
    {
        [SqlPrimaryKey]
        public Guid Identifier { get; set; }

        [SqlNotNull, MaximumSize(128)]
        public string Name { get; set; }

        public int Age { get; set; }

        [SqlNotNull, MaximumSize(8192), TypeConverter(typeof(SomeOtherClassConverter))]
        public SomeOtherClass Payload { get; set; }
    }

    internal sealed class SomeOtherClassConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(byte[]);
        }

        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destinationType)
        {
            if (destinationType != typeof(byte[]))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            return BitConverter.GetBytes(((SomeOtherClass) value).Content);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }

            int content = BitConverter.ToInt32((byte[]) value, 0);

            return new SomeOtherClass
            {
                Content = content
            };
        }
    }

    internal sealed class SomeOtherClass
    {
        public int Content { get; set; }
    }
}