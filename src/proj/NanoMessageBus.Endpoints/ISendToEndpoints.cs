namespace NanoMessageBus.Endpoints
{
	using System;

	public interface ISendToEndpoints : IDisposable
	{
		void Send(TransportMessage message, params string[] recipients);
	}
}