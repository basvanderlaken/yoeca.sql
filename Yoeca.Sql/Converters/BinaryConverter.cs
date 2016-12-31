using System;
using System.ComponentModel;
using System.Globalization;
using JetBrains.Annotations;

namespace Yoeca.Sql.Converters
{
    internal sealed class BinaryConverter : TypeConverter
    {
        [NotNull]
        private static readonly uint[] s_Lookup32 = CreateLookup32();

        [NotNull]
        private readonly TypeConverter m_InternalConverter;

        public BinaryConverter([NotNull] TypeConverter internalConverter)
        {
            m_InternalConverter = internalConverter;
        }

        [NotNull]
        private static uint[] CreateLookup32()
        {
            var result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2");
                result[i] = s[0] + ((uint) s[1] << 16);
            }
            return result;
        }

        [NotNull]
        private static string ByteArrayToHexViaLookup32([NotNull] byte[] bytes)
        {
            var lookup32 = s_Lookup32;
            var result = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                var val = lookup32[bytes[i]];
                result[2 * i] = (char) val;
                result[2 * i + 1] = (char) (val >> 16);
            }
            return new string(result);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(byte[]);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }

            var content = value as byte[];

            if (content != null)
            {
                return m_InternalConverter.ConvertFrom(value);
            }

            var stringValue = value as string;

            if (stringValue != null)
            {
                string binaryContent = stringValue.TrimStart('0').TrimStart('x', 'X').Trim('\'');
                byte[] buffer = StringToByteArray(binaryContent);

                return m_InternalConverter.ConvertFrom(buffer);
            }

            throw new NotSupportedException("Unable to convert binary data.");
        }

        [NotNull]
        public static byte[] StringToByteArray([NotNull] string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        public override object ConvertTo(
            [NotNull] ITypeDescriptorContext context,
            [NotNull] CultureInfo culture,
            [NotNull] object value,
            Type destinationType)
        {
            object result = m_InternalConverter.ConvertTo(value, typeof(byte[]));

            var bytes = result as byte[];
            if (bytes != null)
            {
                return "x'" + ByteArrayToHexViaLookup32(bytes) + "'";
            }

            return result;
        }
    }
}