namespace NanoMessageBus.Queues.Msmq
{
	using System.Messaging;
	using Transport;

	internal static class MessageQueueFactory
	{
		public static MessageQueue OpenConnection(this string address)
		{
			if (string.IsNullOrEmpty(address))
				throw new MessagingException(MsmqMessages.MissingQueueAddress);

			return new MessageQueue(address.NormalizeAddress())
			{
				MessageReadPropertyFilter = BuildFilter()
			};
		}
		private static MessagePropertyFilter BuildFilter()
		{
			var filter = new MessagePropertyFilter();
			filter.SetAll();
			return filter;
		}
	}
}