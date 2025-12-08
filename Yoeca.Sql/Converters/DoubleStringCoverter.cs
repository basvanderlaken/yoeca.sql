using System;
using System.ComponentModel;
using System.Globalization;

namespace Yoeca.Sql.Converters
{
    internal class DoubleStringCoverter : StringConverter
    {
        public override object ConvertTo(
            ITypeDescriptorContext? context,
            CultureInfo? culture,
            object? value,
            Type destinationType)
        {
            if (value == null)
            {
                return "NULL";
            }

            if (value is double doubleValue)
            {
                return doubleValue.ToString("G17", CultureInfo.InvariantCulture);
            }

            return "NULL";
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value == null)
            {
                return 0.0D;
            }

            return double.Parse((string) value);
        }
    }

    internal class DecimalStringCoverter : StringConverter
    {
        public override object ConvertTo(
            ITypeDescriptorContext? context,
            CultureInfo? culture,
            object? value,
            Type destinationType)
        {
            if (value == null)
            {
                return "NULL";
            }

            if (value is decimal decimalValue)
            {
                return decimalValue.ToString("G17", CultureInfo.InvariantCulture);
            }

            return "NULL";
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value == null)
            {
                return 0m;
            }

            return decimal.Parse((string)value);
        }
    }
}
