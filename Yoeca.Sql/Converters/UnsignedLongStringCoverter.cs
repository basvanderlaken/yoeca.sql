using System;
using System.ComponentModel;
using System.Globalization;

namespace Yoeca.Sql.Converters
{
    internal class UnsignedLongStringCoverter : StringConverter
    {
        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destinationType)
        {
            return ((ulong) value).ToString(CultureInfo.InvariantCulture);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return 0UL;
            }

            return ulong.Parse((string) value);
        }
    }
}