namespace NanoMessageBus.Core
{
	using System;

	/// <summary>
	/// Indicates the ability to forward poison messages to the configured poison message queue.
	/// </summary>
	/// <remarks>
	/// Object instances which implement this interface must be designed to be multi-thread safe.
	/// </remarks>
	public interface IHandlePoisonMessages
	{
		/// <summary>
		/// Gets a value indicating whether the message provided is a poison message.
		/// </summary>
		/// <param name="message">The message to be evaluated.</param>
		/// <returns>If the message has been previously handled is now considered poison, returns true; otherwise false.</returns>
		bool IsPoison(TransportMessage message);

		/// <summary>
		/// Indicates to the poison message handler that processing was successful
		/// </summary>
		/// <param name="message">The message that was processed successfully.</param>
		void ClearFailures(TransportMessage message);

		/// <summary>
		/// Forwards the message provided to the configured poison message queue after too many failures.
		/// </summary>
		/// <param name="message">The failed message to forward.</param>
		/// <param name="exception">The exception resulting from handling the message.</param>
		void HandleFailure(TransportMessage message, Exception exception);
	}
}