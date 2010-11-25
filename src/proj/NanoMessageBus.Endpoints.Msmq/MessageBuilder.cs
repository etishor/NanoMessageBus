namespace NanoMessageBus.Endpoints
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Messaging;
	using Serialization;

	internal static class MessageBuilder
	{
		private const string LabelFormat = "NMB:{0}:{1}";

		public static PhysicalMessage Deserialize(this Message message, ISerializeMessages serializer)
		{
			return (PhysicalMessage)serializer.Deserialize(message.BodyStream);
		}

		public static Message BuildMsmqMessage(this PhysicalMessage message, Stream serialized)
		{
			return new Message
			{
				Label = message.GetLabel(),
				BodyStream = serialized,
				Recoverable = message.Persistent,
				TimeToBeReceived = message.GetTimeToBeReceived(),
			};
		}
		private static string GetLabel(this PhysicalMessage message)
		{
			var messages = message.LogicalMessages;
			return LabelFormat.FormatWith(messages.Count, messages.First().GetType().FullName);
		}
		private static TimeSpan GetTimeToBeReceived(this PhysicalMessage message)
		{
			if (message.TimeToLive == TimeSpan.MaxValue || message.TimeToLive == TimeSpan.Zero)
				return MessageQueue.InfiniteTimeout;

			return message.TimeToLive;
		}
	}
}