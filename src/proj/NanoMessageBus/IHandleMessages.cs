namespace NanoMessageBus
{
	/// <summary>
	/// Indicates the ability to handle an incoming message of the specified type.
	/// </summary>
	/// <typeparam name="T">The type of message to be handled.</typeparam>
	/// <remarks>
	/// In general, objects instances which implement this interface should be single threaded,
	/// but depending upon custom IoC container wire-up configuration and/or handler registration,
	/// a specific instance which implements this interface may be designed to be multi-thread safe
	/// allowing it to be shared across threads.  A single-threaded handler will typically have the
	/// lifespan of a single, logical message whereas a multi-threaded handler may have a variable
	/// lifespan dependening upon the container wire-up registration.
	/// </remarks>
	public interface IHandleMessages<T>
	{
		/// <summary>
		/// Handles the message provided.
		/// </summary>
		/// <param name="message">The message to be handled.</param>
		void Handle(T message);
	}
}