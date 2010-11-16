namespace NanoMessageBus.Endpoints.Serialization
{
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;

	public class BinaryMessageSerializer : ISerializeMessages
	{
		private readonly IFormatter formatter = new BinaryFormatter();

		public virtual void Serialize(object message, Stream output)
		{
			this.formatter.Serialize(output, message);
		}
		public virtual object Deserialize(Stream input)
		{
			return this.formatter.Deserialize(input);
		}
	}
}