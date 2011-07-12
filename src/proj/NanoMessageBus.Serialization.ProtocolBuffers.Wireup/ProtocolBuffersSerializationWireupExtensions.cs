
namespace NanoMessageBus
{
    using NanoMessageBus.Wireup;
    using NanoMessageBus.Serialization;

    public static class ProtocolBuffersSerializationWireupExtensions
    {
        public static SerializationWireup WithJsonSerializer(this SerializationWireup wireup)
        {
            return wireup.CustomSerializer(new ProtocolBufferSerializer());
        }
    }
}
