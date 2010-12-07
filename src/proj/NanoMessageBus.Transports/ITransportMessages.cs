namespace NanoMessageBus.Transports
{
	using System;

	/// <summary>
	/// Indicates the ability to transport inbound and outbound messages.
	/// </summary>
	/// <remarks>
	/// Object instances which implement this interface must be designed to be multi-thread safe.
	/// </remarks>
	public interface ITransportMessages : IDisposable
	{
		/// <summary>
		/// Starts the transport listening for new messages to receive.
		/// </summary>
		void StartListening();
		
		/// <summary>
		/// Stops the transport listening for new messages.
		/// </summary>
		void StopListening();

		/// <summary>
		/// Sends the message provided to the set of receipients indicated.
		/// </summary>
		/// <param name="message">The message to be sent.</param>
		/// <param name="recipients">The set of addresses for the interested recipients.</param>
		void Send(TransportMessage message, params Uri[] recipients);
	}
}