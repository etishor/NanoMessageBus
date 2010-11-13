namespace NanoMessageBus.Transport
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class MessageBusException : Exception
	{
		public MessageBusException()
		{
		}
		public MessageBusException(string message)
			: base(message)
		{
		}
		public MessageBusException(string message, Exception inner)
			: base(message, inner)
		{
		}
		protected MessageBusException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}