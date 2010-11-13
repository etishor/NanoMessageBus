namespace NanoMessageBus.Transport
{
	using Core;

	public interface IEnqueueMessage
	{
		void Enqueue(PhysicalMessage message);
	}
}