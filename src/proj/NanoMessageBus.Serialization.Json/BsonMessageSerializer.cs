namespace NanoMessageBus.Serialization
{
    using System;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;

    public class BsonMessageSerializer : SerializerBase
    {
        private readonly JsonSerializer serializer;

        public BsonMessageSerializer(JsonSerializer customSerializer)
        {
            if (customSerializer == null)
            {
                throw new ArgumentNullException("customSerializer");
            }

            this.serializer = customSerializer;
        }

        public BsonMessageSerializer()
            : this(JsonMessageSerializer.CreateDefaultSerializer())
        {
        }

        protected override void SerializeMessage(Stream output, object message)
        {
            using (var bsonWriter = new BsonWriter(output))
                this.serializer.Serialize(bsonWriter, message);
        }
        protected override object DeserializeMessage(Stream input)
        {
            using (var reader = new BsonReader(input))
                return this.serializer.Deserialize(reader);
        }
    }
}