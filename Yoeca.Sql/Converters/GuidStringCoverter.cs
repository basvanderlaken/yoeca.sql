using System;
using System.ComponentModel;
using System.Globalization;

namespace Yoeca.Sql.Converters
{
    internal class GuidStringCoverter : StringConverter
    {
        public override object ConvertTo(
            ITypeDescriptorContext? context,
            CultureInfo? culture,
            object? value,
            Type destinationType)
        {
            if (value is null)
            {
                return null!;
            }

            if (value is Guid valueGuid)
            {
                return valueGuid.ToString("N");
            }

            return string.Empty;
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is null || value is DBNull)
            {
                return null!;
            }

            if (value is string stringValue)
            {
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    return null!;
                }

                return Guid.Parse(stringValue);
            }

            if (value is Guid guidValue)
            {
                return guidValue;
            }

            return null!;
        }
    }
}
