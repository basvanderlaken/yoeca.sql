using System;
using System.ComponentModel;
using System.Globalization;

namespace Yoeca.Sql
{
    internal class DoubleStringCoverter : StringConverter
    {
        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destinationType)
        {
            return ((int) value).ToString("G17", CultureInfo.InvariantCulture);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return 0.0D;
            }
            return double.Parse((string) value);
        }
    }
}