namespace NanoMessageBus.Core
{
	public interface ITransformIncomingMessages
	{
		object Transform(object message);
	}
}