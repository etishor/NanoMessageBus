namespace NanoMessageBus.Serialization
{
	using System.IO;
	using System.Runtime.Serialization;

	/// <summary>
	/// Indicates the ability to serialize and deserialize a message.
	/// </summary>
	/// <remarks>
	/// Object instances which implement this interface must be designed to be multi-thread safe.
	/// </remarks>
	public interface ISerializeMessages
	{
		/// <summary>
		/// Serializes the message provided into the stream specified.
		/// </summary>
		/// <param name="output">The output stream into which all serialized bytes should be written.</param>
		/// <param name="message">The message payload to be serialized.</param>
		/// <exception cref="SerializationException" />
		void Serialize(Stream output, object message);

		/// <summary>
		/// Deserializes the stream specified into an object graph.
		/// </summary>
		/// <param name="input">The stream from which all serialized bytes are to be read.</param>
		/// <returns>If successful, returns a fully reconstituted message.</returns>
		/// <exception cref="SerializationException" />
		object Deserialize(Stream input);
	}
}