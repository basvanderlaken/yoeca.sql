using ProtoBuf;

namespace Yoeca.Sql.Tests.Integration
{
    [ProtoContract]
    public sealed class Payload
    {
        [ProtoMember(1)]
        public int ValueA { get; set; }

        [ProtoMember(2)]
        public int ValueB { get; set; }
    }
}