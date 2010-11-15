namespace NanoMessageBus.Endpoints
{
	using System.IO;

	public interface ISerializeMessages
	{
		byte[] Serialize(object message);
		object Deserialize(Stream payload);
	}
}