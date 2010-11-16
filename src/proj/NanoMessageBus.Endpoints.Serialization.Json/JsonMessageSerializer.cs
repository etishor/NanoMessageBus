namespace NanoMessageBus.Endpoints.Serialization.Json
{
	using System.IO;
	using Newtonsoft.Json;

	public class JsonMessageSerializer : ISerializeMessages
	{
		private readonly JsonSerializer serializer = new JsonSerializer
		{
			TypeNameHandling = TypeNameHandling.Objects
		};

		public virtual Stream Serialize(object message)
		{
			var stream = new MemoryStream();
			var streamWriter = new StreamWriter(stream);
			var jsonWriter = new JsonTextWriter(streamWriter);
			this.serializer.Serialize(jsonWriter, message);
			jsonWriter.Flush();
			streamWriter.Flush();
			stream.Position = 0;

			return stream;
		}
		public virtual object Deserialize(Stream payload)
		{
			using (var streamReader = new StreamReader(payload))
			using (var jsonReader = new JsonTextReader(streamReader))
				return this.serializer.Deserialize(jsonReader);
		}
	}
}