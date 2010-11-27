namespace NanoMessageBus.Serialization
{
	using System.IO;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Bson;

	public class BsonMessageSerializer : SerializerBase
	{
		private readonly JsonSerializer serializer = new JsonSerializer
		{
			TypeNameHandling = TypeNameHandling.Objects
		};

		protected override void SerializeMessage(object message, Stream output)
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