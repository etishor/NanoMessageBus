namespace NanoMessageBus.Endpoints.Serialization.Json
{
	using System.IO;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Bson;

	public class BsonMessageSerializer : ISerializeMessages
	{
		private readonly JsonSerializer serializer = new JsonSerializer
		{
			TypeNameHandling = TypeNameHandling.Objects
		};

		public virtual Stream Serialize(object message)
		{
			var stream = new MemoryStream();
			using (var bsonWriter = new BsonWriter(stream))
			{
				this.serializer.Serialize(bsonWriter, message);
				bsonWriter.Flush();
				stream.Position = 0;
				return stream;
			}
		}
		public virtual object Deserialize(Stream payload)
		{
			using (var reader = new BsonReader(payload))
				return this.serializer.Deserialize(reader);
		}
	}
}