namespace NanoMessageBus.Transport
{
	using Core;

	public interface IDequeueMessages
	{
		bool HasMessage { get; }
		PhysicalMessage Dequeue();
	}
}