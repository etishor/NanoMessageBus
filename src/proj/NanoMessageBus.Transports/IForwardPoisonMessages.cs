namespace NanoMessageBus.Transports
{
	using System;

	/// <summary>
	/// Indicates the ability to forward poison messages to the configured poison message queue.
	/// </summary>
	public interface IForwardPoisonMessages
	{
		/// <summary>
		/// Marks the message provided as successful.
		/// </summary>
		/// <remarks>
		/// This method is used to clear any kind of tracking performed if a message has failed previously.
		/// </remarks>
		/// <param name="message">The message which was processed successfully.</param>
		void MarkAsSuccessful(TransportMessage message);

		/// <summary>
		/// Forwards the message provided to the configured poison message queue.
		/// </summary>
		/// <param name="message">The failed message to forward.</param>
		/// <param name="exception">The exception resulting from handling the message, if any.</param>
		void Forward(TransportMessage message, Exception exception);
	}
}