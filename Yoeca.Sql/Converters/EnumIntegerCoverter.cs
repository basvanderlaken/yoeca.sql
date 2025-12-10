using System;
using System.ComponentModel;
using System.Globalization;

namespace Yoeca.Sql.Converters
{
    internal class EnumIntegerCoverter : TypeConverter
    {
        private readonly Type m_EnumType;

        private readonly Array m_EnumValues;

        public EnumIntegerCoverter(Type enumType)
        {
            m_EnumType = enumType;
            m_EnumValues = Enum.GetValues(enumType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            return destinationType == typeof(string) || destinationType == typeof(int);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(int);
        }

        public override object? ConvertTo(
            ITypeDescriptorContext? context,
            CultureInfo? culture,
            object? value,
            Type destinationType)
        {
            if (value == null)
            {
                value = 0;
            }

            var integerValue = (int) value;

            if (destinationType == typeof(string))
            {
                return integerValue.ToString();
            }

            if (destinationType == typeof(int))
            {
                return integerValue;
            }

            throw new InvalidOperationException("Unable to convert " + value + " to " + destinationType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            int integerValue = 0;
            if (value is string)
            {
                integerValue = int.Parse((string) value);
            }

            if (value is int)
            {
                integerValue = (int) value;
            }

            return Enum.ToObject(m_EnumType, integerValue);
        }
    }
}