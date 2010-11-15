namespace NanoMessageBus.Transport
{
	public interface ITransportMessages
	{
		void Send(PhysicalMessage envelope, params string[] recipientAddress);
	}
}