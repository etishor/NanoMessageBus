namespace NanoMessageBus.Transports
{
	using System;

	/// <summary>
	/// Indicates the ability to forward poison messages to the configured poison message queue.
	/// </summary>
	public interface IForwardPoisonMessages : IHandleMessages<TransportMessage>
	{
		/// <summary>
		/// Forwards the message provided to the configured poison message queue.
		/// </summary>
		/// <param name="message">The failed message to forward.</param>
		/// <param name="exception">The exception resulting from handling the message, if any.</param>
		void HandleFailure(TransportMessage message, Exception exception);
	}
}