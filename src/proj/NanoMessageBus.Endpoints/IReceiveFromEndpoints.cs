namespace NanoMessageBus.Endpoints
{
	using System;

	public interface IReceiveFromEndpoints : IDisposable
	{
		PhysicalMessage Receive();
	}
}