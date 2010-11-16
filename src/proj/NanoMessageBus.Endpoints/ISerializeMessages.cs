namespace NanoMessageBus.Endpoints
{
	using System.IO;

	public interface ISerializeMessages
	{
		Stream Serialize(object message);
		object Deserialize(Stream payload);
	}
}