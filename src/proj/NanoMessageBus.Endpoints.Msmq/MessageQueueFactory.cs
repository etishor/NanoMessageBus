namespace NanoMessageBus.Endpoints.Msmq
{
	using System.Messaging;
	using Transport;

	internal static class MessageQueueFactory
	{
		public static MessageQueue OpenForReceive(this string address)
		{
			var queue = address.Open(QueueAccessMode.Receive);
			queue.MessageReadPropertyFilter.SetAll();
			return queue;
		}
		public static MessageQueue OpenForSend(this string address)
		{
			return address.Open(QueueAccessMode.Send);
		}
		private static MessageQueue Open(this string address, QueueAccessMode accessMode)
		{
			if (string.IsNullOrEmpty(address))
				throw new EndpointException(MsmqMessages.MissingQueueAddress);

			return new MessageQueue(address.ToQueuePath(), accessMode);
		}
	}
}