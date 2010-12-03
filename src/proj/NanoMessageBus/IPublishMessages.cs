namespace NanoMessageBus
{
	/// <summary>
	/// Indicates the ability to publish a message to message subscribers.
	/// </summary>
	public interface IPublishMessages
	{
		/// <summary>
		/// Publishes the collection of messages provided to the subscribers of the first message.
		/// </summary>
		/// <param name="messages">The messages to be published.</param>
		void Publish(params object[] messages);
	}
}