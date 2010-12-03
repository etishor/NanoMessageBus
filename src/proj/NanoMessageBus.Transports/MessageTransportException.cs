namespace NanoMessageBus.Transports
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Represents an error during communication with the transport.
	/// </summary>
	[Serializable]
	public class MessageTransportException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the MessageTransportException class.
		/// </summary>
		public MessageTransportException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the MessageTransportException class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public MessageTransportException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the MessageTransportException class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="inner">The exception that is the cause of the current exception.</param>
		public MessageTransportException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// Initializes a new instance of the MessageTransportException class.
		/// </summary>
		/// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The StreamingContext that holds contextual information about the source or destination.</param>
		protected MessageTransportException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}