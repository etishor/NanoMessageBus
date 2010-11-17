namespace NanoMessageBus.Endpoints.Msmq
{
	using System.IO;
	using System.Messaging;
	using Serialization;

	internal static class MessageBuilder
	{
		public static PhysicalMessage BuildMessage(this Message message, ISerializeMessages serializer)
		{
			return (PhysicalMessage)serializer.Deserialize(message.BodyStream);
		}

		public static Message BuildMessage(this PhysicalMessage message, Stream serialized)
		{
			// TODO: Label and TimeToBeReceived
			return new Message
			{
				BodyStream = serialized,
				Recoverable = message.Durable
			};
		}
	}
}