namespace NanoMessageBus.Transport
{
	using Core;

	public interface ITransportMessages
	{
		void Send(PhysicalMessage envelope, params string[] recipientAddress);
	}
}