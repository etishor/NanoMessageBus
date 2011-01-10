namespace NanoMessageBus
{
	/// <summary>
	/// Indicates the ability to publish a message to message subscribers.
	/// </summary>
	/// <remarks>
	/// Object instances which implement this interface must be designed to be multi-thread safe.
	/// </remarks>
	public interface IPublishMessages
	{
		/// <summary>
		/// Publishes the series of messages provided to the subscribers of the first message.
		/// </summary>
		/// <param name="messages">The messages to be published.</param>
		void Publish(params object[] messages);
	}
}