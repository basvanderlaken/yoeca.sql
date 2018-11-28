using System;
using System.ComponentModel;
using System.Globalization;

namespace Yoeca.Sql.Converters
{
    internal class LongStringCoverter : StringConverter
    {
        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destinationType)
        {
            return ((long) value).ToString(CultureInfo.InvariantCulture);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return 0L;
            }

            return long.Parse((string) value);
        }
    }
}