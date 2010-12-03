namespace NanoMessageBus.Endpoints
{
	using System;

	public interface IReceiveFromEndpoints : IDisposable
	{
		string EndpointAddress { get; }
		TransportMessage Receive();
	}
}