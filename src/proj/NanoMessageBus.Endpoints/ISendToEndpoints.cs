namespace NanoMessageBus.Endpoints
{
	using System;

	/// <summary>
	/// Indicates the ability to send a message to an endpoint.
	/// </summary>
	/// <remarks>
	/// Object instances which implement this interface must be designed to be multi-thread safe.
	/// </remarks>
	public interface ISendToEndpoints : IDisposable
	{
		/// <summary>
		/// Sends the message provided to the recipients indicated.
		/// </summary>
		/// <param name="message">The message to be sent.</param>
		/// <param name="recipients">The collection of recipients interested in the message provided.</param>
		void Send(TransportMessage message, params string[] recipients);
	}
}