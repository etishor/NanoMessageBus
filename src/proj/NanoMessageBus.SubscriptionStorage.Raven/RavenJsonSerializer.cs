// -----------------------------------------------------------------------
// <copyright file="RavenJsonSerializer.cs" company="Recognos Romania">
// {RecognosCopyrightTextPlaceholder}
// </copyright>
// -----------------------------------------------------------------------

namespace NanoMessageBus.Serialization
{
    using System;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Raven.Json.Linq;

    public class RavenJsonSerializer : SerializerBase
    {
        private readonly JsonSerializer serializer;

        public RavenJsonSerializer(JsonSerializer customSerializer)
        {
            if (customSerializer == null)
            {
                throw new ArgumentNullException("customSerializer");
            }

            this.serializer = customSerializer;
        }

        public RavenJsonSerializer()
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
                ContractResolver = resolver
            };
        }

        protected override void SerializeMessage(Stream output, object message)
        {
            var streamWriter = new StreamWriter(output);
            var jsonWriter = new JsonTextWriter(streamWriter);
            RavenJToken.FromObject(message, this.serializer).WriteTo(jsonWriter);
            jsonWriter.Flush();
            streamWriter.Flush();
        }
        protected override object DeserializeMessage(Stream input)
        {
            using (var streamReader = new StreamReader(input))
            using (var jsonReader = new JsonTextReader(streamReader))
            using (RavenJTokenReader tokenReader = new RavenJTokenReader(RavenJToken.ReadFrom(jsonReader)))
            {
                return serializer.Deserialize(tokenReader);
            }
        }
    }
}
