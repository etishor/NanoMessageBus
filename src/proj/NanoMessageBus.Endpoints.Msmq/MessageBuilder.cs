namespace NanoMessageBus.Endpoints.Msmq
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Messaging;
	using Core;
	using Transport;

	internal static class MessageBuilder
	{
		public static PhysicalMessage BuildPhysicalMessage(this Message message, ISerializeMessages serializer)
		{
			return new PhysicalMessage
			{
				MessageId = Guid.Parse(message.Id), // TODO, this should probably come from the serialized payload?
				CorrelationId = Guid.Parse(message.CorrelationId), // TODO, also from the payload?
				ReturnAddress = message.ResponseQueue.ToQueueAddress(),
				Durable = message.Recoverable,
				Expiration = message.Expiration(),
				Headers = message.Headers(serializer),
				LogicalMessages = message.LogicalMessages(serializer)
			};
		}

		private static DateTime Expiration(this Message message)
		{
			var ttl = message.TimeToBeReceived;
			return ttl.Ticks == 0 ? DateTime.MaxValue : message.SentTime + message.TimeToBeReceived;
		}

		private static ICollection LogicalMessages(this Message message, ISerializeMessages serializer)
		{
			message.BodyStream.Position = 0;
			return (ICollection)serializer.Deserialize(message.BodyStream);
		}

		private static IDictionary<string, string> Headers(this Message message, ISerializeMessages serializer)
		{
			using (var stream = new MemoryStream(message.Extension))
				return (IDictionary<string, string>)serializer.Deserialize(stream);
		}

		public static Message BuildMsmqMessage(this PhysicalMessage message, ISerializeMessages serializer)
		{
			return new Message(); // TODO
		}
	}
}