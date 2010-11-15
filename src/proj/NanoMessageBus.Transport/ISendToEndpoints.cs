namespace NanoMessageBus.Transport
{
	using System;
	using Core;

	public interface ISendToEndpoints : IDisposable
	{
		void Send(PhysicalMessage message, params string[] recipients);
	}
}