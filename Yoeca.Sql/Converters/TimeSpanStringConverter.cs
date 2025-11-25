using System;
using System.ComponentModel;
using System.Globalization;

namespace Yoeca.Sql.Converters
{
    internal sealed class TimeSpanStringConverter : StringConverter
    {
        private const string TimeFormat = "d\\ hh\\:mm\\:ss\\.ffffff";

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            return destinationType == typeof(string);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(TimeSpan);
        }

        public override object? ConvertTo(
            ITypeDescriptorContext? context,
            CultureInfo? culture,
            object? value,
            Type destinationType)
        {
            if (value is TimeSpan span)
            {
                return span.ToString(TimeFormat, CultureInfo.InvariantCulture);
            }

            return null;
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value == null)
            {
                return TimeSpan.Zero;
            }

            if (value is TimeSpan span)
            {
                return span;
            }

            if (TimeSpan.TryParseExact((string)value, TimeFormat, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return TimeSpan.Parse((string)value, CultureInfo.InvariantCulture);
        }
    }
}
