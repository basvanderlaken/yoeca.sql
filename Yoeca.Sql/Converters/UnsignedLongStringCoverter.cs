using System;
using System.ComponentModel;
using System.Globalization;

namespace Yoeca.Sql.Converters
{
    internal class UnsignedLongStringCoverter : StringConverter
    {
        public override object ConvertTo(
            ITypeDescriptorContext? context,
            CultureInfo? culture,
            object? value,
            Type destinationType)
        {
            if (value is ulong longValue)
            {
                return longValue.ToString(CultureInfo.InvariantCulture);
            }
            
            return string.Empty;
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value == null)
            {
                return 0UL;
            }

            return ulong.Parse((string) value);
        }
    }
}