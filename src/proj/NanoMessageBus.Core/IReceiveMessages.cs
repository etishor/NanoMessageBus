namespace NanoMessageBus.Core
{
	public interface IReceiveMessages : IMessageContext
	{
		void Receive(PhysicalMessage message);
	}
}