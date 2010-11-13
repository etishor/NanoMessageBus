namespace NanoMessageBus.Transport
{
	using Core;

	public delegate void MessageReceivedHandler(PhysicalMessage envelope);

	public interface ITransportMessages
	{
		void Send(PhysicalMessage envelope, params string[] recipientAddress);
		event MessageReceivedHandler Received;
	}
}