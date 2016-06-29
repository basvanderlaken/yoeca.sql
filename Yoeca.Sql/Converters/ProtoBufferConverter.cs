using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using ProtoBuf;

namespace Yoeca.Sql.Converters
{
    internal sealed class ProtoBufferConverter<TContract> : TypeConverter
        where TContract : class
    {
        public ProtoBufferConverter()
        {
            if (typeof(TContract).GetCustomAttribute<ProtoContractAttribute>() == null)
            {
                throw new NotSupportedException("Type " + typeof(TContract).FullName + " is not a valid protcontract");
            }
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(byte[]);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var bytes = value as byte[];

            if (bytes == null)
            {
                throw new InvalidOperationException("Unable to deserialize.");
            }
            var stream = new MemoryStream(bytes);
            stream.Seek(0, SeekOrigin.Begin);
            return Serializer.Deserialize<TContract>(stream);
        }

        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destinationType)
        {
            TContract valueAsResult = value as TContract;
            if (valueAsResult == null || destinationType != typeof(byte[]))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, valueAsResult);

                byte[] buffer = stream.ToArray();

                if (buffer.Length != stream.Length)
                {
                    var result = new byte[buffer.Length];

                    Array.Copy(buffer, result, result.Length);
                    return result;
                }

                return buffer;
            }
        }
    }
}