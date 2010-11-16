namespace NanoMessageBus.Endpoints.Serialization.Json
{
	using System.IO;
	using System.Text;
	using Newtonsoft.Json;

	public class JsonSerializer : ISerializeMessages
	{
		public Stream Serialize(object message)
		{
			// TODO: optimize this
			var settings = new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All
			};

			var json = JsonConvert.SerializeObject(message, Formatting.Indented, settings);

			return new MemoryStream(Encoding.UTF8.GetBytes(json));
		}
		public object Deserialize(Stream payload)
		{
			using (var reader = new StreamReader(payload, Encoding.UTF8))
				return JsonConvert.DeserializeObject(reader.ReadToEnd());
		}
	}
}