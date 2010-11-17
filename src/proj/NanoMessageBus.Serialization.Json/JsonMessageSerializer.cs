namespace NanoMessageBus.Serialization
{
	using System.IO;
	using Logging;
	using Newtonsoft.Json;

	public class JsonMessageSerializer : ISerializeMessages
	{
		private static readonly ILog Log = LogFactory.BuildLogger(typeof(JsonMessageSerializer));
		private readonly JsonSerializer serializer = new JsonSerializer
		{
			TypeNameHandling = TypeNameHandling.Objects
		};

		public virtual void Serialize(object message, Stream output)
		{
			Log.Verbose(Diagnostics.Serializing, message.GetType());
			var streamWriter = new StreamWriter(output);
			var jsonWriter = new JsonTextWriter(streamWriter);
			this.serializer.Serialize(jsonWriter, message);
			jsonWriter.Flush();
			streamWriter.Flush();
		}
		public virtual object Deserialize(Stream input)
		{
			Log.Verbose(Diagnostics.Deserializing, input.Length);
			using (var streamReader = new StreamReader(input))
			using (var jsonReader = new JsonTextReader(streamReader))
				return this.serializer.Deserialize(jsonReader);
		}
	}
}