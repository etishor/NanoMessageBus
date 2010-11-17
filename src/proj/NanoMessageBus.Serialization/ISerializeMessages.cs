namespace NanoMessageBus.Endpoints
{
	using System.IO;

	public interface ISerializeMessages
	{
		void Serialize(object message, Stream output);
		object Deserialize(Stream input);
	}
}