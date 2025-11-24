using System;
using System.ComponentModel;
using System.Globalization;

namespace Yoeca.Sql.Converters
{
    internal sealed class DateOnlyStringConverter : StringConverter
    {
        private const string DateFormat = "yyyy-MM-dd";

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            return destinationType == typeof(string);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(DateTime);
        }

        public override object? ConvertTo(
            ITypeDescriptorContext? context,
            CultureInfo? culture,
            object? value,
            Type destinationType)
        {
            if (value is DateOnly date)
            {
                return date.ToString(DateFormat, CultureInfo.InvariantCulture);
            }

            return null;
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value == null)
            {
                return DateOnly.MinValue;
            }

            if (value is DateTime dateTime)
            {
                return DateOnly.FromDateTime(dateTime);
            }

            return DateOnly.ParseExact((string)value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }
    }
}
