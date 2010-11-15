namespace NanoMessageBus.Transport
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class MessageTransportException : Exception
	{
		public MessageTransportException()
		{
		}
		public MessageTransportException(string message)
			: base(message)
		{
		}
		public MessageTransportException(string message, Exception inner)
			: base(message, inner)
		{
		}
		protected MessageTransportException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}