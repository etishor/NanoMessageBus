namespace NanoMessageBus.Transports
{
	using System;

	public interface ITransportMessages : IDisposable
	{
		void StartListening();
		void StopListening();

		void Send(TransportMessage message, params string[] recipients);
	}
}