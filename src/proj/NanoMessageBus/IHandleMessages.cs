namespace NanoMessageBus
{
	/// <summary>
	/// Indicates the ability to handle an incoming message of the specified type.
	/// </summary>
	/// <typeparam name="T">The type of message to be handled.</typeparam>
	public interface IHandleMessages<T>
	{
		/// <summary>
		/// Handles the message provided.
		/// </summary>
		/// <param name="message">The message to be handled.</param>
		void Handle(T message);
	}
}