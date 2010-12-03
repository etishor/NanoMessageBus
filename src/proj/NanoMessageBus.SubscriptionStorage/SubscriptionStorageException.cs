namespace NanoMessageBus.SubscriptionStorage
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Represents an error during communication with the underlying subscription storage.
	/// </summary>
	[Serializable]
	public class SubscriptionStorageException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the SubscriptionStorageException class.
		/// </summary>
		public SubscriptionStorageException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the SubscriptionStorageException class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public SubscriptionStorageException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the SubscriptionStorageException class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="inner">The exception that is the cause of the current exception.</param>
		public SubscriptionStorageException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// Initializes a new instance of the SubscriptionStorageException class.
		/// </summary>
		/// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The StreamingContext that holds contextual information about the source or destination.</param>
		protected SubscriptionStorageException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}