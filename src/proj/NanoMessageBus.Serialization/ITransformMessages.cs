namespace NanoMessageBus.Serialization
{
	/// <summary>
	/// Indicates the ability to transform a message into another type.
	/// </summary>
	/// <remarks>
	/// Object instances which implement this interface must be designed to be multi-thread safe.
	/// </remarks>
	public interface ITransformMessages
	{
		/// <summary>
		/// Transforms the message into another type of message.
		/// </summary>
		/// <param name="message">The message to be transformed.</param>
		/// <returns>Returns a new message based on the one provided.</returns>
		object Transform(object message);
	}
}