namespace NanoMessageBus.Endpoints
{
	using System;

	public interface ISendToEndpoints : IDisposable
	{
		void Send(PhysicalMessage message, params string[] recipients);
	}
}