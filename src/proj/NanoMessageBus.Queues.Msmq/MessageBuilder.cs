namespace NanoMessageBus.Queues.Msmq
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Messaging;
	using Core;
	using Transport;

	internal static class MessageBuilder
	{
		public static PhysicalMessage BuildPhysicalMessage(
			this Message message, MessageQueue queue, ISerializeMessages serializer)
		{
			return new PhysicalMessage
			{
				MessageId = Guid.Parse(message.Id), // TODO, this should probably come from the serialized payload?
				CorrelationId = Guid.Parse(message.CorrelationId), // TODO, also from the payload?
				ReturnAddress = message.ResponseQueue.ToQueueAddress(),
				DestinationAddress = queue.ToQueueAddress(),
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

		private static ICollection LogicalMessages(
			this Message message, ISerializeMessages serializer)
		{
			var buffer = new byte[message.BodyStream.Length];
			message.BodyStream.Position = 0;
			message.BodyStream.Read(buffer, 0, buffer.Length);
			return (ICollection)serializer.Deserialize(buffer);
		}

		private static IDictionary<string, string> Headers(
			this Message message, ISerializeMessages serializer)
		{
			return (IDictionary<string, string>)serializer.Deserialize(message.Extension);
		}
	}
}