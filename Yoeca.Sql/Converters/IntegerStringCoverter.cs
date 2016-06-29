using System;
using System.ComponentModel;
using System.Globalization;

namespace Yoeca.Sql
{
    internal class IntegerStringCoverter : StringConverter
    {
        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destinationType)
        {
            return ((int) value).ToString(CultureInfo.InvariantCulture);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return 0;
            }
            return Int32.Parse((string) value);
        }
    }
}