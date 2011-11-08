namespace NanoMessageBus.Serialization
{
    using System;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class JsonMessageSerializer : SerializerBase
    {
        private readonly JsonSerializer serializer;

        public JsonMessageSerializer(JsonSerializer customSerializer)
        {
            if (customSerializer == null)
            {
                throw new ArgumentNullException("customSerializer");
            }

            this.serializer = customSerializer;
        }

        public JsonMessageSerializer()
            : this(CreateDefaultSerializer())
        {
        }

        public static JsonSerializer CreateDefaultSerializer()
        {
            DefaultContractResolver resolver = new DefaultContractResolver();
            // allow json.net to use private setter
            resolver.DefaultMembersSearchFlags |= System.Reflection.BindingFlags.NonPublic;

            return new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Objects,
                MissingMemberHandling = MissingMemberHandling.Error, // catch possible deserialization errors
                ContractResolver = resolver
            };
        }

        protected override void SerializeMessage(Stream output, object message)
        {
            var streamWriter = new StreamWriter(output);
            var jsonWriter = new JsonTextWriter(streamWriter);
            this.serializer.Serialize(jsonWriter, message);
            jsonWriter.Flush();
            streamWriter.Flush();
        }
        protected override object DeserializeMessage(Stream input)
        {
            using (var streamReader = new StreamReader(input))
            using (var jsonReader = new JsonTextReader(streamReader))
                return this.serializer.Deserialize(jsonReader);
        }
    }
}