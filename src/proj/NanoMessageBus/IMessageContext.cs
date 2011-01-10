namespace NanoMessageBus
{
	/// <summary>
	/// Provides the current context surrounding the incoming message being handled.
	/// </summary>
	/// <remarks>
	/// Object instances which implement this interface should be designed to be single threaded and
	/// should not be shared between threads.  The object lifetime of instances will be the receipt
	/// of a single EnvelopeMessage.
	/// </remarks>
	public interface IMessageContext
	{
		/// <summary>
		/// Gets the current message being handled.
		/// </summary>
		EnvelopeMessage CurrentMessage { get; }

		/// <summary>
		/// Gets a value indicating whether dispatching the current message to handlers should continue.
		/// </summary>
		bool ContinueProcessing { get; }

		/// <summary>
		/// Defers additional processing of the incoming envelope message until a later time.
		/// </summary>
		void DeferMessage();

		/// <summary>
		/// Stops all additional processing of the incoming envelope message and drops the message.
		/// </summary>
		void DropMessage();
	}
}