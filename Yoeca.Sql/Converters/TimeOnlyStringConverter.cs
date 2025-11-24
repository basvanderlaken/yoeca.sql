using System;
using System.ComponentModel;
using System.Globalization;

namespace Yoeca.Sql.Converters
{
    internal sealed class TimeOnlyStringConverter : StringConverter
    {
        private const string TimeFormat = "HH:mm:ss";

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
            if (value is TimeOnly time)
            {
                return time.ToString(TimeFormat, CultureInfo.InvariantCulture);
            }

            return null;
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value == null)
            {
                return TimeOnly.MinValue;
            }

            if (value is TimeSpan timeSpan)
            {
                return TimeOnly.FromTimeSpan(timeSpan);
            }

            return TimeOnly.ParseExact((string)value, TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }
    }
}
