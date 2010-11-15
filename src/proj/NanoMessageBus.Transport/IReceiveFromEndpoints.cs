namespace NanoMessageBus.Transport
{
	using System;
	using Core;

	public interface IReceiveFromEndpoints : IDisposable
	{
		event EventHandler MessageAvailable;
		PhysicalMessage Receive();
	}
}