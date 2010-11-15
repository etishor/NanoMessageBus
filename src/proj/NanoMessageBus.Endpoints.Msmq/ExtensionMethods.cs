namespace NanoMessageBus.Endpoints.Msmq
{
	using System;
	using System.Messaging;

	internal static class ExtensionMethods
	{
		public static MessageQueueTransactionType GetInboundTransactionType(this bool transactional)
		{
			return transactional ? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.None;
		}
		public static MessageQueueTransactionType GetOutboundTransactionType(this bool transactional)
		{
			return transactional ? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.Single;
		}

		public static TimeSpan Seconds(this int seconds)
		{
			return new TimeSpan(0, 0, 0, seconds);
		}

		public static PhysicalMessage MessageNoLongerAvailable(this MsmqAdapter queue)
		{
			return null;
		}
	}
}