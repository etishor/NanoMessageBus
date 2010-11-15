namespace NanoMessageBus.Core
{
	public interface ITransformOutgoingMessages
	{
		object Transform(object message);
	}
}