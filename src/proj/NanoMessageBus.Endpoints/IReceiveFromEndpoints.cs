namespace NanoMessageBus.Endpoints
{
	using System;

	/// <summary>
	/// Indicates the ability to receive a message from an endpoint.
	/// </summary>
	/// <remarks>
	/// Object instances which implement this interface must be designed to be multi-thread safe.
	/// </remarks>
	public interface IReceiveFromEndpoints : IDisposable
	{
		/// <summary>
		/// Gets the address of the endpoint.
		/// </summary>
		string EndpointAddress { get; }

		/// <summary>
		/// Receives a message from the endpoint, if any.
		/// </summary>
		/// <returns>Returns the message received, if any; otherwise null is returned.</returns>
		TransportMessage Receive();
	}
}