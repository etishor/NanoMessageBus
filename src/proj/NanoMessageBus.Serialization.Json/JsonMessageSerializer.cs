namespace NanoMessageBus.Serialization
{
	using System.IO;
	using Newtonsoft.Json;

	public class JsonMessageSerializer : SerializerBase
	{
		private readonly JsonSerializer serializer = new JsonSerializer
		{
			TypeNameHandling = TypeNameHandling.Objects
		};

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