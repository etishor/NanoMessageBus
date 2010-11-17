namespace NanoMessageBus.Endpoints.Serialization.Json
{
	using System.IO;
	using Logging;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Bson;

	public class BsonMessageSerializer : ISerializeMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(BsonMessageSerializer));
		private readonly JsonSerializer serializer = new JsonSerializer
		{
			TypeNameHandling = TypeNameHandling.Objects
		};

		public virtual void Serialize(object message, Stream output)
		{
			Log.Verbose(Messages.Serializing, message.GetType());
			using (var bsonWriter = new BsonWriter(output))
				this.serializer.Serialize(bsonWriter, message);
		}
		public virtual object Deserialize(Stream input)
		{
			Log.Verbose(Messages.Deserializing, input.Length);
			using (var reader = new BsonReader(input))
				return this.serializer.Deserialize(reader);
		}
	}
}