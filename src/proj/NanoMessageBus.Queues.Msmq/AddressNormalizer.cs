namespace NanoMessageBus.Queues.Msmq
{
	using System.Messaging;

	internal static class AddressNormalizer
	{
		public static string ToQueueAddress(this MessageQueue queue)
		{
			return string.Empty; // TODO: translate \\machine\QueueName to QueueName@machine
		}
		public static string ToQueuePath(this string address)
		{
			return string.Empty; // TODO: translate QueueName[@machine] to QueueName@machine
		}
	}
}