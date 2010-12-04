namespace NanoMessageBus.Core
{
	using System;

	/// <summary>
	/// Indicates the ability to forward poison messages to the configured poison message queue.
	/// </summary>
	public interface IHandlePoisonMessages
	{
		/// <summary>
		/// Indicates to the poison message handler that process was successful.
		/// </summary>
		/// <param name="message">The message that was processed successfully.</param>
		void HandleSuccess(TransportMessage message);

		/// <summary>
		/// Forwards the message provided to the configured poison message queue after too many failures.
		/// </summary>
		/// <param name="message">The failed message to forward.</param>
		/// <param name="exception">The exception resulting from handling the message.</param>
		void HandleFailure(TransportMessage message, Exception exception);
	}
}