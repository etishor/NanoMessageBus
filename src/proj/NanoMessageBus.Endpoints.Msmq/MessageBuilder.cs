namespace NanoMessageBus.Endpoints.Msmq
{
	using System.Messaging;

	internal static class MessageBuilder
	{
		public static PhysicalMessage BuildMessage(this Message message, ISerializeMessages serializer)
		{
			return (PhysicalMessage)serializer.Deserialize(message.BodyStream);
		}

		public static Message BuildMessage(this PhysicalMessage message, ISerializeMessages serializer)
		{
			return new Message { Body = serializer.Serialize(message) };
		}
	}
}