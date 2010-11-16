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

		public virtual void Serialize(object message, Stream output)
		{
			var streamWriter = new StreamWriter(output);
			var jsonWriter = new JsonTextWriter(streamWriter);
			this.serializer.Serialize(jsonWriter, message);
			jsonWriter.Flush();
			streamWriter.Flush();
		}
		public virtual object Deserialize(Stream input)
		{
			using (var streamReader = new StreamReader(input))
			using (var jsonReader = new JsonTextReader(streamReader))
				return this.serializer.Deserialize(jsonReader);
		}
	}
}