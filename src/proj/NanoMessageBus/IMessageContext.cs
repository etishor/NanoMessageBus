namespace NanoMessageBus
{
	/// <summary>
	/// Provides current context surrounding the incoming message being handled.
	/// </summary>
	public interface IMessageContext
	{
		/// <summary>
		/// Gets the current message being handled.
		/// </summary>
		TransportMessage CurrentMessage { get; }

		/// <summary>
		/// Gets a value indicating whether dispatching the current message to handlers should continue.
		/// </summary>
		bool ContinueProcessing { get; }

		/// <summary>
		/// Defers additional processing of the incoming transport message until a later time.
		/// </summary>
		void DeferMessage();

		/// <summary>
		/// Stops all additional processing of the incoming transport message and drops the message.
		/// </summary>
		void DropMessage();
	}
}