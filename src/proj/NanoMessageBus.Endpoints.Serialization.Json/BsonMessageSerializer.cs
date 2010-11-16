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

		public virtual void Serialize(object message, Stream output)
		{
			using (var bsonWriter = new BsonWriter(output))
				this.serializer.Serialize(bsonWriter, message);
		}
		public virtual object Deserialize(Stream input)
		{
			using (var reader = new BsonReader(input))
				return this.serializer.Deserialize(reader);
		}
	}
}