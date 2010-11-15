namespace NanoMessageBus.Transport
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class EndpointException : Exception
	{
		public EndpointException()
		{
		}
		public EndpointException(string message)
			: base(message)
		{
		}
		public EndpointException(string message, Exception inner)
			: base(message, inner)
		{
		}
		protected EndpointException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}