namespace NanoMessageBus.Serialization
{
	using System.IO;

	public interface ISerializeMessages
	{
		void Serialize(Stream output, object message);
		object Deserialize(Stream input);
	}
}