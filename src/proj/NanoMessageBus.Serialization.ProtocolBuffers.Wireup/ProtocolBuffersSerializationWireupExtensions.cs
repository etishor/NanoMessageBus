
namespace NanoMessageBus.Serialization.ProtocolBuffers.Wireup
{
    using NanoMessageBus.Wireup;

    public static class ProtocolBuffersSerializationWireupExtensions
    {
        public static SerializationWireup WithJsonSerializer(this SerializationWireup wireup)
        {
            return wireup.CustomSerializer(new ProtocolBufferSerializer());
        }
    }
}
