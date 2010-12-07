namespace NanoMessageBus
{
	/// <summary>
	/// Indicates the ability to send messages to registered recipients.
	/// </summary>
	/// <remarks>
	/// Object instances which implement this interface must be designed to be multi-thread safe.
	/// </remarks>
	public interface ISendMessages
	{
		/// <summary>
		/// Sends the collection of messages provided to the registered recipients of the first message.
		/// </summary>
		/// <param name="messages">The messages to send.</param>
		void Send(params object[] messages);

		/// <summary>
		/// Sends the collection of messages provided back to the return address of the current message context.
		/// </summary>
		/// <param name="messages">The messages to sent back to the return address.</param>
		void Reply(params object[] messages);
	}
}