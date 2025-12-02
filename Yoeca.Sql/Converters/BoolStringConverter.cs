using System;
using System.ComponentModel;
using System.Globalization;

namespace Yoeca.Sql.Converters
{
    internal sealed class BoolStringConverter : StringConverter
    {
        public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "1" : "0";
            }

            return string.Empty;
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value == null)
            {
                return null;
            }

            var stringValue = value.ToString();

            return string.Equals(stringValue, "1", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(stringValue, "true", StringComparison.OrdinalIgnoreCase);
        }
    }
}
