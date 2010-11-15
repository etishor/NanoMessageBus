namespace NanoMessageBus.Endpoints.Msmq
{
	using System;
	using System.Messaging;
	using Core;

	internal static class ExtensionMethods
	{
		public static MessageQueueTransactionType GetTransactionalType(this bool transactional)
		{
			return transactional ? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.None;
		}

		public static TimeSpan Seconds(this int seconds)
		{
			return new TimeSpan(0, 0, 0, seconds);
		}

		public static PhysicalMessage MessageNoLongerAvailable(this MessageQueue queue)
		{
			return null;
		}
	}
}