namespace NanoMessageBus.Transport
{
	using Core;

	public interface ITransportMessages
	{
		void Send(PhysicalMessage envelope, params string[] recipientAddress);

		// TODO: do we really need this?
		void Start();
		void Stop();
	}
}