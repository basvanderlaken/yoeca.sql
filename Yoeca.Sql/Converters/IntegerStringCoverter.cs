using System;
using System.ComponentModel;
using System.Globalization;

namespace Yoeca.Sql.Converters
{
    internal class IntegerStringCoverter : StringConverter
    {
        public override object ConvertTo(
            ITypeDescriptorContext? context,
            CultureInfo? culture,
            object? value,
            Type destinationType)
        {
            if (value is int intValue)
            {
                return intValue.ToString(CultureInfo.InvariantCulture);
            }
            return string.Empty;
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value == null)
            {
                return 0;
            }

            return int.Parse((string) value);
        }
    }
}