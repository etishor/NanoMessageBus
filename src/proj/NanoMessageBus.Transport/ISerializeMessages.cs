namespace NanoMessageBus.Transport
{
	public interface ISerializeMessages
	{
		byte[] Serialize(object message);
		object Deserialize(byte[] payload);
	}
}