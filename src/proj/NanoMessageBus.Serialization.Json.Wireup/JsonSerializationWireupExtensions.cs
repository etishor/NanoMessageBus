
namespace NanoMessageBus.Serialization.Json.Wireup
{
    using NanoMessageBus.Wireup;

    public static class JsonSerializationWireupExtensions
    {
        public static SerializationWireup WithJsonSerializer(this SerializationWireup wireup)
        {
            return wireup.CustomSerializer(new JsonMessageSerializer());
        }

        public static SerializationWireup WithBsonSerializer(this SerializationWireup wireup)
        {
            return wireup.CustomSerializer(new JsonMessageSerializer());
        }
    }
}
