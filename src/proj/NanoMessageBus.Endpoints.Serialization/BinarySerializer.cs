namespace NanoMessageBus.Endpoints.Serialization
{
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;

	public class BinarySerializer : ISerializeMessages
	{
		private readonly IFormatter formatter = new BinaryFormatter();

		public virtual Stream Serialize(object message)
		{
			var stream = new MemoryStream();
			this.formatter.Serialize(stream, message);
			return stream;
		}
		public virtual object Deserialize(Stream payload)
		{
			return this.formatter.Deserialize(payload);
		}
	}
}