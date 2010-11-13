namespace NanoMessageBus.Transport
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class MessagingException : Exception
	{
		public MessagingException()
		{
		}
		public MessagingException(string message)
			: base(message)
		{
		}
		public MessagingException(string message, Exception inner)
			: base(message, inner)
		{
		}
		protected MessagingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}