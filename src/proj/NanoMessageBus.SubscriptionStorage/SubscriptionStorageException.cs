namespace NanoMessageBus.SubscriptionStorage
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class SubscriptionStorageException : Exception
	{
		public SubscriptionStorageException()
		{
		}
		public SubscriptionStorageException(string message)
			: base(message)
		{
		}
		public SubscriptionStorageException(string message, Exception inner)
			: base(message, inner)
		{
		}
		protected SubscriptionStorageException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}