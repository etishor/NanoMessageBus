namespace NanoMessageBus.Endpoints
{
	using System;

	public interface IReceiveFromEndpoints : IDisposable
	{
		event EventHandler MessageAvailable;
		PhysicalMessage Receive();
	}
}